using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(SaveLoadGameDataController))]
    static class Patch_LoadPostfix
    {
        public static string savePath;

        [HarmonyPatch("StartNewGame")]
        public static void Prefix(SaveLoadGameDataController __instance)
        {
            savePath = __instance.savePath;
        }

        [HarmonyPatch("LoadGameDataCoroutine", MethodType.Enumerator)]
        public static void Postfix(SaveLoadGameDataController __instance, ref bool __result)
        {
            // This makes sure there are no more elements to go through (makes sure its a true postfix!)
            if (__result)
            {
                return;
            }

            // Setup visual log stuff
            Singleton<SceneMessagePanelUI>.Instance.SetLoadingText("Loading Saved Custom Buildables...", 97);
            SaveLoadUtility.quicklog("Starting Custom Load Round!", true);

            // Make sure the diretory exists!
            if (!Directory.Exists(savePath))
            {
                SaveLoadUtility.quicklog("The save directory does not exist!", true);
                return;
            }


            // Path based stuff... Is there a file?
            string path = Path.Combine(savePath, "CustomSaveData.json");
            if (!File.Exists(path))
            {
                SaveLoadUtility.quicklog("The CustomSaveData.json file does not exist. Skipped loading.", false);
                return;
            }

            string JSON;
            try
            {
                // Read the file!
                JSON = Utils.ReadFile(path);
            }
            catch (Exception ex)
            {
                SaveLoadUtility.quicklog("Failed to read JSON! Error: " + ex.Message, true);
                return;
            }
            if (string.IsNullOrEmpty(JSON))
            {
                SaveLoadUtility.quicklog("Empty JSON string!", true);
                return;
            }

            // Now we know that the string has something...
            CustomItemSerializableWrapper customItemSerializableWrapper = null;
            try
            {
                customItemSerializableWrapper = JsonConvert.DeserializeObject<CustomItemSerializableWrapper>(JSON);
            }
            catch (Exception ex)
            {
                SaveLoadUtility.quicklog("JSON deserialized failed! Error: " + ex.Message, true);
                return;
            }

            if (customItemSerializableWrapper == null)
            {
                SaveLoadUtility.quicklog("JSON deserialized object is null!!", true);
                return;
            }

            SaveLoadUtility.quicklog("A total of " + customItemSerializableWrapper.customItemSerializables.Count + " custom items were found and will be loaded/changed", false);

            // Now we know the Item info is valid
            DynamicSimpleArray<PlaceableItem> itemsList = Singleton<BuildingController>.Instance.allItemsArray;
            foreach (CustomItemSerializable customItem in customItemSerializableWrapper.customItemSerializables)
            {
                Vector3 customPostion = new Vector3(customItem.postion[0], customItem.postion[1], customItem.postion[2]);
                float spriteRotation = customItem.spriteRotation;
                float itemRotation = customItem.itemRotation;

                foreach (PlaceableItem worldItem in itemsList.ToList())
                {
                    // Required to be the same
                    if (!Vector3.Equals(worldItem.gameObject.transform.position, customPostion))
                    {
                        continue;
                    }

                    if (!int.Equals(worldItem.Floor, customItem.floor))
                    {
                        continue;
                    }

                    // Auxilary checks, to make sure
                    if (!string.Equals(worldItem.ReferenceID, customItem.referenceID))
                    {
                        SaveLoadUtility.quicklog("What? The reference ID is different, but the postion and floor are the same? huh?", true);
                    }

                    // We know now that they are the same! We need to find the itemIndex of the mod from the id now
                    for (int i = 0; i < ItemModSourceCreator.Instance.buildableMods.Count; i++)
                    {
                        ItemMod itemMod = ItemModSourceCreator.Instance.buildableMods[i] as ItemMod;
                        if (itemMod == null)
                        {
                            continue;
                        }

                        if (!string.Equals(itemMod.id, customItem.modId))
                        {
                            if (i != ItemModSourceCreator.Instance.buildableMods.Count - 1)
                            {
                                continue;
                            }

                            // Now the problem is there isn't a matching ID!
                            SaveLoadUtility.quicklog("[Buildable Problem] A custom item is in the processs of being loaded, but a custom item with the id in the save file doesn't exist. " +
                                "This probably means that a mod was uninstalled or its ID was changed. To fix this, either re-install the mod or revert any changes made to a mods id. " +
                                "The id being looked for is " + customItem.modId + ". For help, contact Humoresque.", false);
                            continue;
                        }

                        // The id is the same!
                        if (itemMod.useRandomRotation)
                        {
                            ItemCreator.Instance.ConvertItemToCustom(worldItem.gameObject, i, true, spriteRotation, itemRotation);
                            break;
                        }

                        ItemCreator.Instance.ConvertItemToCustom(worldItem.gameObject, i, false, spriteRotation, itemRotation);
                        break;
                    }
                }
            }

            Singleton<SceneMessagePanelUI>.Instance.SetLoadingText(LocalizationManager.GetLocalizedValue("SaveLoadGameDataController.cs.key.almost-done"), 100);
            SaveLoadUtility.quicklog("Custom items finished loading, without any errors!", true);
        }
    }
}
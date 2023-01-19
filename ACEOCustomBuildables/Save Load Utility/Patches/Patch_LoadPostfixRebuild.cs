using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using static ACEOCustomBuildables.JSONManager;

namespace ACEOCustomBuildables.Patches
{
    [HarmonyPatch(typeof(SaveLoadGameDataController))]
    static class Patch_LoadPostfixRebuild
    {
        /*private static string savePath;
        private static CustomItemSerializableWrapper customItemSerializableWrapper = null;

        [HarmonyPatch("LoadGameDataCoroutine")]
        public static void Prefix(SaveLoadGameDataController __instance)
        {
            if (TemplateManager.getAllTemplates())
            {
                ACEOCustomBuildables.Log("[Mod Nuetral] Started loading mod info!");
                JSONManager.importJSON();
                if (JSONManager.buildableMods.Count != 0)
                {
                    ItemManager.clearBuildables();
                    UIManager.clearUI();
                    ACEOCustomBuildables.loadMods();
                    ACEOCustomBuildables.buildableCreationProccessInProgress = true;
                }
            }
            savePath = __instance.savePath;




            // Other part
            SaveLoadUtility.quicklog("Starting Custom Load Round!", true);
            // Make sure the diretory exists!
            if (!Directory.Exists(savePath))
            {
                SaveLoadUtility.quicklog("The save directory does not exist!", true);
                return;
            }


            // Path based stuff... Is there a file?
            string path = savePath + "\\CustomSaveData.json";
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
            CustomItemSerializableWrapper wrapper = null;
            try
            {
                wrapper = JsonConvert.DeserializeObject<CustomItemSerializableWrapper>(JSON);
            }
            catch (Exception ex)
            {
                SaveLoadUtility.quicklog("JSON deserialized failed! Error: " + ex.Message, true);
                return;
            }

            if (wrapper == null)
            {
                SaveLoadUtility.quicklog("JSON deserialized object is null!!", true);
                return;
            }
            customItemSerializableWrapper = wrapper;

            SaveLoadUtility.quicklog("A total of " + customItemSerializableWrapper.customItemSerializables.Count + " custom items were found and will be loaded/changed", false);
        }

        [HarmonyPatch("LoadGameDataCoroutine", MethodType.Enumerator)]
        public static void Postfix(SaveLoadGameDataController __instance, ref bool __result)
        {
            // This makes sure there are no more elements to go through (makes sure its a true postfix!)
            if (__result)
            {
                return;
            }
            /* Retired
            SaveLoadUtility.quicklog("Starting Custom Load Round!", true);
            // Make sure the diretory exists!
            if (!Directory.Exists(savePath))
            {
                SaveLoadUtility.quicklog("The save directory does not exist!", true);
                return;
            }


            // Path based stuff... Is there a file?
            string path = savePath + "\\CustomSaveData.json";
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
                Quaternion roation = new Quaternion(customItem.rotation[0], customItem.rotation[1], customItem.rotation[2], customItem.rotation[3]);
                
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
                        SaveLoadUtility.quicklog("What? The reference ID is different, but the postion and floor aren't? huh", true);
                        continue;
                    }

                    // We know now that they are the same! We need to find the index of the mod from the id now
                    for (int i = 0; i < buildableMods.Count; i++)
                    {
                        if (!string.Equals(JSONManager.buildableMods[i].id, customItem.modId))
                        {
                            continue;
                        }

                        // The id is the same!
                        if (customItem.useRandomRotation)
                        {
                            ItemManager.convertItemToCustom(worldItem.gameObject, i, true, roation);
                            break;
                        }

                        ItemManager.convertItemToCustom(worldItem.gameObject, i, false, roation);
                    }
                }
            }
            SaveLoadUtility.quicklog("Custom items loaded successfully!", true);
        }

        [HarmonyPatch(typeof(PlaceableItem), "SetObjectFromSerializer")]
        public static void Postfix(PlaceableItem __instance)
        {
            foreach (CustomItemSerializable customItem in customItemSerializableWrapper.customItemSerializables)
            {
                Vector3 customPostion = new Vector3(customItem.postion[0], customItem.postion[1], customItem.postion[2]);
                Quaternion roation = new Quaternion(customItem.rotation[0], customItem.rotation[1], customItem.rotation[2], customItem.rotation[3]);

                // Required to be the same
                if (!Vector3.Equals(__instance.gameObject.transform.position, customPostion))
                {
                    continue;
                }

                if (!int.Equals(__instance.Floor, customItem.floor))
                {
                    continue;
                }

                // Auxilary checks, to make sure
                if (!string.Equals(__instance.ReferenceID, customItem.referenceID))
                {
                    SaveLoadUtility.quicklog("What? The reference ID is different, but the postion and floor aren't? huh", true);
                    continue;
                }

                // We know now that they are the same! We need to find the index of the mod from the id now
                for (int i = 0; i < buildableMods.Count; i++)
                {
                    if (!string.Equals(JSONManager.buildableMods[i].id, customItem.modId))
                    {
                        continue;
                    }

                    // The id is the same!
                    if (customItem.useRandomRotation)
                    {
                        ItemManager.convertItemToCustom(__instance.gameObject, i, true, roation);
                        break;
                    }

                    ItemManager.convertItemToCustom(__instance.gameObject, i, false, roation);
                }
            }
        }*/
    }
}
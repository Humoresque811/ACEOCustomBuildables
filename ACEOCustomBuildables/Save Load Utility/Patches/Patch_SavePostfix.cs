using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(SaveLoadGameDataController))]
    static class Patch_SavePostfix
    {
        private static string inputSavePath;

        [HarmonyPatch("SaveGameData")]
        public static void Prefix(SaveLoadGameDataController __instance, string savePath)
        {
            // This is to get the appropriate variable, savePath, since the other patch can't get it
            inputSavePath = savePath;
        }

        [HarmonyPatch("SaveGameData", MethodType.Enumerator)]
        public static void Postfix(SaveLoadGameDataController __instance, ref bool __result)
        {
            // This makes sure there are no more elements to go through (makes sure its a true postfix!)
            if (__result)
            {
                return;
            }

            // Configure Game World before saving
            SaveLoadUtility.quicklog("Starting Custom Save Round!", true);
            SaveLoadUtility.makeGameReadyForSaving();
            SaveLoadUtility.getAllBuildablesArrays();


            // Custom code!
            try
            {
                foreach (PlaceableItem item in SaveLoadUtility.itemArray.ToList())
                {
                    if (!item.gameObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent itemCustomInfo))
                    {
                        continue;
                    }

                    // So if it is custom, then...
                    CustomItemSerializable customItemSerializable = SaveLoadUtility.setSerializableInfo(itemCustomInfo.index, item);
                    if (customItemSerializable != null)
                    {
                        SaveLoadUtility.JSONInfoArray.Add(customItemSerializable);
                        continue;
                    }
                    SaveLoadUtility.quicklog("Custom Serializable item was null!", false);
                }

                if (string.IsNullOrEmpty(inputSavePath))
                {
                    inputSavePath = Singleton<SaveLoadGameDataController>.Instance.saveName;
                }

                SaveLoadUtility.createJSON(inputSavePath);
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("Error in custom save code! error: " + ex.Message, true);
            }


            // Revert Game World after saving  <---------------------------------------- IMPORTANT
            SaveLoadUtility.revetGameAfterSaving();
            SaveLoadUtility.JSONInfoArray.Clear();
        }
    }
}
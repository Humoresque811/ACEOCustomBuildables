using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using System.Linq;
using Steamworks;

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
                    if (!item.gameObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent serializableComponent))
                    {
                        continue;
                    }

                    if (serializableComponent.itemIndex != serializableComponent.nullInt)
                    {
                        SerializeItems(serializableComponent.itemIndex, item);
                    }

                    if (serializableComponent.tileableIndex != serializableComponent.nullInt)
                    {
                        SerializeTileables(serializableComponent.tileableIndex, item);
                    }
                }

                SerializeFloors();

                if (string.IsNullOrEmpty(inputSavePath))
                {
                    inputSavePath = Singleton<SaveLoadGameDataController>.Instance.saveName;
                }

                SaveLoadUtility.CreateJSON(inputSavePath);
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("Error in custom save code! error: " + ex.Message, true);
            }


            // Revert Game World after saving  <---------------------------------------- IMPORTANT
            SaveLoadUtility.revetGameAfterSaving();
            SaveLoadUtility.itemJSONList = new List<CustomItemSerializable>();
            SaveLoadUtility.floorJSONList = new List<CustomFloorSerializable>();
            SaveLoadUtility.tileableJSONList = new List<CustomTileableSerializable>();
        }

        private static void SerializeItems(in int itemIndex, in PlaceableItem item)
        {
            CustomItemSerializable customItemSerializable = SaveLoadUtility.SetItemSerializableInfo(itemIndex, item);
            if (customItemSerializable != null)
            {
                SaveLoadUtility.itemJSONList.Add(customItemSerializable);
                return;
            }
            SaveLoadUtility.quicklog("Custom Serializable item was null!", false);
        }

        private static void SerializeTileables(in int tileableIndex, in PlaceableItem tileable)
        {
            CustomTileableSerializable customTileableSerializable = SaveLoadUtility.SetTileableSerializableInfo(tileableIndex, tileable);
            if (customTileableSerializable != null)
            {
                SaveLoadUtility.tileableJSONList.Add(customTileableSerializable);
                return;
            }
            SaveLoadUtility.quicklog("Custom Serializable tileable was null!", false);
        }

        private static void SerializeFloors()
        {
            foreach (MergedTile mergedTile in SaveLoadUtility.zonesArray.ToList())
            {
                if (!mergedTile.gameObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent serializableComponent))
                {
                    continue;
                }

                if (serializableComponent.floorIndex == serializableComponent.nullInt)
                {
                    continue;
                }

                // So if it is custom, then...
                CustomFloorSerializable customFloorSerializable = SaveLoadUtility.SetFloorSeializableInfo(serializableComponent.floorIndex, mergedTile);
                if (customFloorSerializable != null)
                {
                    SaveLoadUtility.floorJSONList.Add(customFloorSerializable);
                    continue;
                }
                SaveLoadUtility.quicklog("Custom Serializable floor was null!", false);
            }
        }
    }
}
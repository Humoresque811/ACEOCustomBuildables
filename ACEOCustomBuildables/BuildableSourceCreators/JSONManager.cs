using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class JSONManager : MonoBehaviour
    {

        // THIS CODE IS NO LONGER IN USE! DO NOT REFERENCE! Will be removed before release of 1.1

        //Setup Variables
        private static List<ItemMod> itemMods = new List<ItemMod>();
        private static List<FloorMod> floorMods = new List<FloorMod>();
        private static List<string> modPaths = new List<string>();

        private static void importJSON()
        {
            throw new NotImplementedException();

            // Clears mods
#pragma warning disable CS0162 // Unreachable code detected
            itemMods = new List<ItemMod>();
            floorMods = new List<FloorMod>();
            ItemCreator.Instance.buildables = new List<GameObject>();

            importJSONFromFolder();

            if (modPaths.Count <= 0)
            {
                ACEOCustomBuildables.Log("[Mod Success] (re-)Imported " + itemMods.Count + " JSON file(s) from just the buildables folder");
                return;
            }

            for (int i = 0; i < modPaths.Count; i++)
            {
                importJSONFromFolder(modPaths[i]);
            }
            ACEOCustomBuildables.Log("[Mod Success] (re-)Imported " + itemMods.Count + " JSON file(s) from mods and the buildables folder");
        }

        private static void importJSONFromFolder(string path = "")
        {
            throw new NotImplementedException();

            string internalLog = "";
            bool giveUsePath = false;
            if (string.IsNullOrEmpty(path))
            {
                path = FileManager.Instance.basePath;
                ACEOCustomBuildables.Log("[Mod Neutral] Started reading the LocalLow buildables folder");
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Neutral] Started reading a folder (Workshop or Native mod folder)");
                giveUsePath = true;
            }

            try
            {

                // Intitial checks
                if (string.IsNullOrEmpty(path))
                {
                    ACEOCustomBuildables.Log("[Mod Error] The path is empty or null... ");
                    return;
                }
                if (!Directory.Exists(path))
                {
                    ACEOCustomBuildables.Log("[Mod Error] There is no directory at the path...");
                    return;
                }

                // Get JSON files in the given folder
                string[] jsonFilePaths = new string[1];
                try
                {
                    jsonFilePaths = Directory.GetFiles(path, "*.json");
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Couldn't get JSON file paths... Error: " + ex.ToString());
                    return;
                }
                internalLog += "\nCompleted getting JSON files";

                // Adds mods
                for (int i = 0; i < jsonFilePaths.Length; i++)
                {
                    if (FileManager.Instance.GetJSONFileContent(jsonFilePaths[i]) == null)
                    {
                        ACEOCustomBuildables.Log("[Mod Error] JSON File Paths is null!");
                        return;
                    }

                    // Make sure the mod being added is enabled
                    ItemMod preLoadedMod = JsonUtility.FromJson<ItemMod>(FileManager.Instance.GetJSONFileContent(jsonFilePaths[i]));
                    if (preLoadedMod.enabled != true)
                    {
                        continue;
                    }

                    // This is an old system that does not work as intended, but doesn't hurt anything, so it is staying for now
                    itemMods.Add(new ItemMod());
                    itemMods[itemMods.Count - 1] = JsonUtility.FromJson<ItemMod>(FileManager.Instance.GetJSONFileContent(jsonFilePaths[i]));

                    if (giveUsePath)
                    {
                        itemMods[itemMods.Count - 1].pathToUse = path;
                    }
                    else
                    {
                        itemMods[itemMods.Count - 1].pathToUse = "";
                    }

                    // Bogus system is called
                    string dialog = bogusNumberScanner(itemMods.Count - 1);
                    if (!string.IsNullOrEmpty(dialog))
                    {
                        DialogPanel.Instance.ShowMessagePanel(dialog, Color.black);
                    }
                }
                internalLog += "\nRead all JSON files and did Bogus checks";

                // Checks
                for (int i = 0; i < itemMods.Count; i++)
                {
                    for (int k = 0; k < itemMods.Count; k++)
                    {
                        if (i == k)
                        {
                            continue;
                        }

                        if (itemMods[i].id != itemMods[k].id)
                        {
                            continue;
                        }

                        ACEOCustomBuildables.Log("[Buildable Error] You have two mods with the same id... This will result in problems with saveload, so the mod called " +
                            itemMods[k].name + " with id " + itemMods[k].id + " was removed.");
                        DialogPanel.Instance.ShowMessagePanel("[Airport CEO Custom Buildables] Your mod \"" + itemMods[k].name + "\" has a duclicate id to mod \"" + itemMods[i].name + "\", so the first mod wasn't loaded.", Color.black);
                        itemMods.RemoveAt(i);
                        break;
                    }
                }
                internalLog += "\nFinished final checks";
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] An error occured while doing a JSON file scan. Info: " +
                    "\nError: " + ex.Message + 
                    "\nPath: " + path + 
                    "\nError Debug Log: " + internalLog);
            }
        }

        // This is used for the logger action in the input checker helper
        private static void Log(string message)
        {
            throw new NotImplementedException();
            ACEOCustomBuildables.Log(message);
        }

        private static string bogusNumberScanner(int index)
        {
            throw new NotImplementedException();
            // For shorter reference
            ItemMod mod = itemMods[index];

            Action<string> Logger = new Action<string>(Log);
            BogusInputHelper.CheckItemMod(itemMods[index], Logger);
            return "";
        }

        private static bool CanCountinueLoading()
        {
            throw new NotImplementedException();
            return itemMods.Count > 0;
        }
	}
}
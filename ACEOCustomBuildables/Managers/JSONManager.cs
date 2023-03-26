using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class JSONManager : MonoBehaviour
    {
        //Setup Variables
        public static List<itemMod> itemMods = new List<itemMod>();
        public static List<floorMod> floorMods = new List<floorMod>();
        public static List<string> modPaths = new List<string>();
        public static string basePath = "";
        public static readonly string pathAddativeBase = "Buildables";
        public static readonly string pathAddativeItems = "Items"; // This will require changing eventually, as there will be multiple

        public static void importJSON()
        {
            // Clears mods
            itemMods = new List<itemMod>();
            floorMods = new List<floorMod>();
            ItemManager.buildableModItems = new List<GameObject>();

            importJSONFromFolder();

            if (modPaths.Count <= 0)
            {
                ACEOCustomBuildables.Log("[Mod Success] (re-)Imported " + JSONManager.itemMods.Count + " JSON file(s) from just the buildables folder");
                return;
            }

            for (int i = 0; i < modPaths.Count; i++)
            {
                importJSONFromFolder(modPaths[i]);
            }
            ACEOCustomBuildables.Log("[Mod Success] (re-)Imported " + JSONManager.itemMods.Count + " JSON file(s) from mods and the buildables folder");
        }

        private static void importJSONFromFolder(string path = "")
        {
            string internalLog = "";
            bool giveUsePath = false;
            if (string.IsNullOrEmpty(path))
            {
                path = basePath;
                ACEOCustomBuildables.Log("[Mod Nuetral] Started reading the LocalLow buildables folder");
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Nuetral] Started reading a folder (Workshop or Native mod folder)");
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
                    if (getJSONFileContent(jsonFilePaths[i]) == null)
                    {
                        ACEOCustomBuildables.Log("[Mod Error] JSON File Paths is null!");
                        return;
                    }

                    // Make sure the mod being added is enabled
                    itemMod preLoadedMod = JsonUtility.FromJson<itemMod>(getJSONFileContent(jsonFilePaths[i]));
                    if (preLoadedMod.enabled != true)
                    {
                        continue;
                    }

                    // This is an old system that does not work as intended, but doesn't hurt anything, so it is staying for now
                    itemMods.Add(new itemMod());
                    itemMods[itemMods.Count - 1] = JsonUtility.FromJson<itemMod>(getJSONFileContent(jsonFilePaths[i]));

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

        private static string getJSONFileContent(string path)
        {
            if (!File.Exists(path))
            {
                ACEOCustomBuildables.Log("[Mod Error] Nothing at getJSONFileContent's provided search path!");
                return null;
            }

            // We know the file exists now!
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }


        private static string bogusNumberScanner(int index)
        {
            // For shorter reference
            itemMod mod = itemMods[index];
            string dialog;

            // These are the important ones in which case the mod needs to be removed
            if (mod.x < 0 || mod.y < 0 || mod.operationCost < 0 || mod.buildCost < 0)
            {
                ACEOCustomBuildables.Log("[Buildable Problem] Your mod called \"" + mod.name + "\" seems to have a negative int value. This may break the game, so the mod has not been loaded.");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a negative int value, so it wasn't loaded.";
                itemMods.RemoveAt(index);
                return dialog;
            }

            // Need the contruction variables to be valid
            if (mod.constructionEnergy <= 0)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" seems to have a construction energy value below or at 0, so it was changed to 50.");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a construction energy value below or at 0, so it was changed to 50.";
                mod.constructionEnergy = 50;
                return dialog;
            }
            if (mod.constructionEnergy > 5000)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" seems to have a too high construction energy value, so it was changed to 500.");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a too high construction energy value, so it was changed to 500.";
                mod.constructionEnergy = 500;
                return dialog;
            }

            if (mod.contractors <= 0)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" seems to have a contracters value below or at 0, so it was changed to 1");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a contracters value below or at 0, so it was changed to 1.";
                mod.contractors = 1;
                return dialog;
            }
            if (mod.contractors > 100)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" seems to have a too high contractors value, so it was changed to 100.");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a too high contractors value, so it was changed to 100.";
                mod.contractors = 100;
                return dialog;
            }

            if (mod.shadowDistance < 0)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" seems to have a shadowDistance value below 0, so it was changed to 0");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a shadowDistance value below 0, so it was changed to 0.";
                mod.shadowDistance = 0;
                return dialog;
            }
            if (mod.shadowTextureSizeMultiplier < 1)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" seems to have a shadowMultiplier value below 1, so it was changed to 1.");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a shadowMultiplier value below 1, so it was changed to 1.";
                mod.shadowTextureSizeMultiplier = 1;
                return dialog;
            }


            // Non critical, both is assumed. Just for good measure
            bool correctItemPlacementAreaInput = false;
            if (mod.itemPlacementArea == "Both" || mod.itemPlacementArea == "Inside" || mod.itemPlacementArea == "Outside" )
            {
                correctItemPlacementAreaInput = true;
            }
            if (!correctItemPlacementAreaInput)
            {
                ACEOCustomBuildables.Log("[Buildable Non-Critical Problem] Your mod called \"" + mod.name + "\" does not have a valid item placment area (It's " + mod.itemPlacementArea + "). Both is assumed. The options for the value are:" +
                    "\n Both \n Inside \n Outside");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" doesn't have an item placement area variable, so \"Both\" was assumed.";
                mod.itemPlacementArea = "Both";
                return dialog;
            }

            // X and Y are not always enforced
            if (mod.bogusOverride)
            {
                return "";
            }


            // I don't want people using big numbers, so I won't let them unless they know coding and can read this file or they ask me. Then I trust them.
            if (mod.x > 16)
            {
                ACEOCustomBuildables.Log("[Buildable Problem] Your mod called \"" + mod.name + "\" seems to have a high \"x\" value. " +
                    "This may break the game, so the mod has not been loaded. Contact Humoresque if you think your mod should work. Sorry!");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has too high of a \"x\" value, so it wasn't loaded.";
                itemMods.RemoveAt(index);
                return dialog;
            }
            if (mod.y > 16)
            {
                ACEOCustomBuildables.Log("[Buildable Problem] Your mod called \"" + mod.name + "\" seems to have a high \"y\" value. " +
                "This may break the game, so the mod has not been loaded. Contact Humoresque if you think your mod should work. Sorry!");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has too high of a \"y\" value, so it wasn't loaded.";
                itemMods.RemoveAt(index);
                return dialog;
            }

            return "";
        }


        /// <summary>
        /// Will get a sprite (png image) from a path using either mod index or mod class
        /// </summary>
        /// <param name="modIndex">The JSONManager mod index to get mod info from</param>
        /// <param name="spriteType">What type of sprite to get ("Texture", "Shadow", or "Icon")</param>
        /// <param name="modClass">A mod class to use instead of mod index</param>
        /// <returns>The loaded Sprite, or a placeholder sprite if an error was encountered</returns>
        public static Sprite getSpriteFromPath(int modIndex, string spriteType, itemMod modClass = null)
        {
            // Local Vars
            itemMod modClassToUse;
            string path;

            // Determine which class to use
            if (modClass != null)
            {
                modClassToUse = modClass;
            }
            else
            {
                modClassToUse = JSONManager.itemMods[modIndex];
            }
            
            // Determine if workshop or not and then use correct path
            if (string.IsNullOrEmpty(modClassToUse.pathToUse))
            {
                path = basePath + Path.DirectorySeparatorChar;
                
            }
            else
            {
                path = modClassToUse.pathToUse + Path.DirectorySeparatorChar;
            }

            // Determine which texture to use
            if (spriteType == "Texture")
            {
                path += modClassToUse.texturePath;
            }
            else if (spriteType == "Shadow")
            {
                if (string.Equals(modClassToUse.shadowPath, "autogenerate"))
                {
                    path += modClassToUse.texturePath;
                }
                else
                {
                    path += modClassToUse.shadowPath;
                }
            }
            else if (spriteType == "Icon")
            {
                path += modClassToUse.iconPath;
            }
            else
            {
                // Return the Fallback image
                return Singleton<DataPlaceholderItems>.Instance.smallPlantIcon;
            }

            // If texture keyword correct, then check if it exists
            if (File.Exists(path))
            {
                // This should be whats returned
                return Utils.LoadImage(path);
            }

            // If it doesn't exist, return this
            return Singleton<DataPlaceholderItems>.Instance.smallPlantIcon;
        }
	}
}
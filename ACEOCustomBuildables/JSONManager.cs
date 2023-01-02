using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class JSONManager : MonoBehaviour
    {
        // Class
        [System.Serializable]
        public class buildableMod
        {
            public bool enabled;
            public string name;
            public string id;
            public string description;
            public int x;
            public int y;
            public string texturePath;
            public string shadowPath;
            public string iconPath;
            public int buildCost;
            public int operationCost;
            public bool bogusOverride;
            public string itemPlacementArea;
            public bool useRandomRotation;
        }


        //Setup Variables
        public static List<buildableMod> buildableMods = new List<buildableMod>();
        public static string basePath = "";

        public static void importJSON()
        {
            if (!Directory.Exists(basePath))
            {
                ACEOCustomBuildables.Log("[Mod Error] There is no directory at the base path...");
                return;
            }

            string[] jsonFilePaths = new string[1];

            try
            {
                jsonFilePaths = Directory.GetFiles(basePath, "*.json");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Couldn't get JSON file paths... Error: " + ex.ToString());
            }

            // Clears mods
            buildableMods = new List<buildableMod>();
            ItemManager.buildableModItems = new List<GameObject>();

            // Re-adds mods
            for (int i = 0; i < jsonFilePaths.Length; i++)
            {
                if (getJsonString(jsonFilePaths[i]) != null)
                {
                    buildableMod test = JsonUtility.FromJson<buildableMod>(getJsonString(jsonFilePaths[i]));
                    if (test.enabled == true)
                    {
                        buildableMods.Add(new buildableMod());
                        resetBuildableModInfo(buildableMods.Count - 1);
                        buildableMods[buildableMods.Count - 1] = JsonUtility.FromJson<buildableMod>(getJsonString(jsonFilePaths[i]));
                        string dialog = bogusNumberScanner(buildableMods.Count - 1);
                        if (!string.IsNullOrEmpty(dialog))
                        {
                            DialogPanel.Instance.ShowMessagePanel(dialog, Color.black);
                        }
                    }
                }
            }

            // Checks
            for (int i = 0; i < buildableMods.Count; i++)
            {
                for (int k = 0; k < buildableMods.Count; k++)
                {
                    if (i != k)
                    {
                        if (buildableMods[i].id == buildableMods[k].id)
                        {
                            ACEOCustomBuildables.Log("[Buildable Error] You have two mods with the same id... This will result in problems with saveload, so the mod called " +
                                buildableMods[k].name + " with id " + buildableMods[k].id + " was removed.");
                            buildableMods.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            ACEOCustomBuildables.Log("[Mod Success] (re-)Imported " + JSONManager.buildableMods.Count + " JSON file(s)");
        }

        private static string getJsonString(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Error] Nothing at search path!");
                return null;
            }
        }


        private static void resetBuildableModInfo(int index)
        {
            buildableMod mod = buildableMods[index];
            mod.name = "Deafult Buildable Name";
            mod.description = "";
            mod.id = "";
            mod.enabled = true;
            mod.x = 1;
            mod.y = 1;
            mod.texturePath = "";
            mod.shadowPath = "";
            mod.iconPath = "";
            mod.buildCost = 10;
            mod.operationCost = 10;
            mod.bogusOverride = false;
            mod.itemPlacementArea = "Both";
            mod.useRandomRotation = false;
        }

        private static string bogusNumberScanner(int index)
        {
            // For shorter reference
            buildableMod mod = buildableMods[index];
            string dialog;

            // These are the important ones in which case the mod needs to be removed
            if (mod.x < 0 || mod.y < 0 || mod.operationCost < 0 || mod.buildCost < 0)
            {
                ACEOCustomBuildables.Log("[Buildable Problem] Your mod called \"" + mod.name + "\" seems to have a negative int value. This may break the game, so the mod has not been loaded.");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has a negative int value, so it wasn't loaded.";
                buildableMods.RemoveAt(index);
                return dialog;
            }
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
                buildableMods.RemoveAt(index);
                return dialog;
            }
            if (mod.y > 16)
            {
                ACEOCustomBuildables.Log("[Buildable Problem] Your mod called \"" + mod.name + "\" seems to have a high \"y\" value. " +
                "This may break the game, so the mod has not been loaded. Contact Humoresque if you think your mod should work. Sorry!");
                dialog = "[Airport CEO Custom Buildables] Your mod \"" + mod.name + "\" has too high of a \"y\" value, so it wasn't loaded.";
                buildableMods.RemoveAt(index);
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

            return "";
        }


        /// <summary>
        /// Will get a sprite (png image) from a path using either mod index or mod class
        /// </summary>
        /// <param name="modIndex">The JSONManager mod index to get mod info from</param>
        /// <param name="spriteType">What type of sprite to get ("Texture", "Shadow", or "Icon")</param>
        /// <param name="modClass">A mod class to use instead of mod index</param>
        /// <returns>The loaded Sprite, or a placeholder sprite if an error was encountered</returns>
        public static Sprite getSpriteFromPath(int modIndex, string spriteType, buildableMod modClass = null)
        {
            // Local Vars
            string path = basePath + "\\";
            buildableMod modClassToUse;

            // Determine which class to use
            if (modClass != null)
            {
                modClassToUse = modClass;
            }
            else
            {
                modClassToUse = JSONManager.buildableMods[modIndex];
            }

            // Determine which texture to use
            if (spriteType == "Texture")
            {
                path += modClassToUse.texturePath;
            }
            else if (spriteType == "Shadow")
            {
                path += modClassToUse.shadowPath;
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
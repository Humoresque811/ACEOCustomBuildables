using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;
using System.IO;

namespace ACEOCustomBuildables
{
    
    [HarmonyPatch(typeof(ModManager), "QueueMods")]
    static class Patch_StartWorkshopLoading
    {
        public static void Postfix(string path)
        {
            ACEOCustomBuildables.Log("[Mod Nuetral] Started loading mods from the workshop!");
            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                // Is there a subfolder with Buildables?
                if (!directories[i].SafeSubstring(directories[i].Length - 10, 10).Equals("Buildables"))
                {
                    continue;
                }

                string fullPath = path + JSONManager.basePathAddativeItems; // Code accodomates future changes to the folder
                if (Workshoputility.checkIfFolderValid(fullPath))
                {
                    JSONManager.modPaths.Add(fullPath);
                }
            }
        }
    }
}

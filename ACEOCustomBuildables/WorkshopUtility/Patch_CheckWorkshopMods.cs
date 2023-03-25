using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using System.IO;

namespace ACEOCustomBuildables
{
    
    [HarmonyPatch(typeof(ModManager), "QueueMods")]
    static class Patch_CheckWorkshopMods
    {   
        public static void Postfix(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                // Is there a subfolder with Buildables?
                if (!directories[i].SafeSubstring(directories[i].Length - 10, 10).Equals("Buildables"))
                {
                    continue;
                }
                ACEOCustomBuildables.Log("[Mod Nuetral] Started loading a mod from the workshop!");

                string fullPath = path + JSONManager.basePathAddativeItems; // Code accodomates future changes to the folder
                if (Workshoputility.checkIfFolderValid(fullPath))
                {
                    JSONManager.modPaths.Add(fullPath);
                    continue;
                }
            }
        }
    }
}

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
                if (!directories[i].SafeSubstring(directories[i].Length - 10, 10).Equals(FileManager.Instance.pathAddativeBase))
                {
                    continue;
                }
                ACEOCustomBuildables.Log("[Mod Neutral] Started loading a mod from the workshop!");

                string basePath = Path.Combine(path, FileManager.Instance.pathAddativeBase);
                if (Workshoputility.CheckIfFolderValid(basePath, out Type modType, out string extendedPath))
                {
                    BuildableClassHelper.GetBuildableSourceCreator(modType, out IBuildableSourceCreator buildableSourceCreator);
                    buildableSourceCreator.modPaths.Add(extendedPath);
                    continue;
                }
            }
        }
    }
}

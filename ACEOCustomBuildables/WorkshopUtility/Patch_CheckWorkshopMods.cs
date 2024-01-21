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
            int modAmount = 0;

            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                // Is there a subfolder with Buildables?
                if (!directories[i].SafeSubstring(directories[i].Length - 10, 10).Equals(FileManager.Instance.pathAddativeBase))
                {
                    continue;
                }
                // Yes there is
                modAmount++;

                string extendedPath = Path.Combine(path, FileManager.Instance.pathAddativeBase);
                if (Workshoputility.CheckIfFolderValid(extendedPath, out List<Type> modTypes, out List<string> fullPaths))
                {
                    for (int k = 0; k < modTypes.Count; k++)
                    {
                        BuildableClassHelper.GetBuildableSourceCreator(modTypes[i], out IBuildableSourceCreator buildableSourceCreator);
                        buildableSourceCreator.modPaths.Add(fullPaths[i]);   
                    }

                    continue;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ACEOCustomBuildables
{
    static class Workshoputility
    {
        public static bool CheckIfFolderValid(string path, out Type modType, out string extendedPath)
        {
            try
            {
                foreach (Type type in FileManager.Instance.buildableTypes.Keys.ToArray())
                {
                    string specificPath = Path.Combine(path, FileManager.Instance.buildableTypes[type].Item1);
                    if (!Directory.Exists(specificPath))
                    {
                        continue;
                    }

                    string[] jsonFiles = Directory.GetFiles(specificPath, "*.json");
                    if (jsonFiles.Length < 1)
                    {
                        continue;
                    }

                    modType = type;
                    extendedPath = specificPath;
                    return true;
                }

                modType = null;
                extendedPath = "";
                return false;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] The code to check if a workshop folder is valid failed. Error: " + ex.Message);
                modType = null;
                extendedPath = "";
                return false;
            }
        }
    }
}

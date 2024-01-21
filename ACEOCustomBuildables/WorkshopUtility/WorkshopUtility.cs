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
        public static bool CheckIfFolderValid(string path, out List<Type> modTypes, out List<string> fullPaths)
        {
            try
            {
                modTypes = new List<Type>();
                fullPaths = new List<string>();
                bool success = false;

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

                    modTypes.Add(type);
                    fullPaths.Add(specificPath);
                    success = true;
                    continue;
                }

                if (success)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] The code to check if a workshop folder is valid failed. Error: " + ex.Message);
                modTypes = new List<Type>();
                fullPaths = new List<string>();
                return false;
            }
        }
    }
}

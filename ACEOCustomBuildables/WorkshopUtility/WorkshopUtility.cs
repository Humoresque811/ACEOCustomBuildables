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
    static class Workshoputility
    {
        /// <summary>
        /// Checks if a folder can contain buildable mods
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if folder is valid, false if not</returns>
        public static bool checkIfFolderValid(string path)
        {
            try
            {
                string[] jsonFiles = Directory.GetFiles(path, "*.json");
                if (jsonFiles.Length > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] The code to check if a workshop folder is valid failed. Error: " + ex.Message);
                return false;
            }
        }
    }
}

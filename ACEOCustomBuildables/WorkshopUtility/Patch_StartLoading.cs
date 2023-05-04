using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;
using System.IO;
using LapinerTools.Steam.Data;

namespace ACEOCustomBuildables
{
    
    [HarmonyPatch(typeof(ModManager), "InitalizeSteamWorkshopMods")]
    static class Patch_StartWorkshopLoading
    {   
        public static void Prefix()
        {
            try
            {
                ACEOCustomBuildables.Log("[Mod Nuetral] Started Loading workshop mods!");
                foreach (Type type in FileManager.Instance.buildableTypes.Keys)
                {
                    BuildableClassHelper.GetBuildableSourceCreator(type, out IBuildableSourceCreator buildableSourceCreator);
                    buildableSourceCreator.modPaths = new List<string>();
                }
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("Error: " + ex.Message);
            }
        }
    }
}

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
            ACEOCustomBuildables.Log("[Mod Nuetral] Started Loading workshop mods!");
            JSONManager.modPaths = new List<string>(); // Clears list to avoid duplicates
        }
    }
}

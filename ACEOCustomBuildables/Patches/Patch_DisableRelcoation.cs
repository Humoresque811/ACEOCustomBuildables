using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(PlaceableObject))]
    [HarmonyPatch("CanBeRelocated")]
    static class Patch_DisableRelcoation
    {   
        public static void Postfix(PlaceableObject __instance, ref bool __result)
        {
            // If the item is custom
            if (__instance.gameObject.transform.TryGetComponent(out CustomItemSerializableComponent mod))
            {
                try
                {
                    __result = false;
                }
                catch (Exception ex)
                {
                    // If it fails log
                    ACEOCustomBuildables.Log("[Mod Error] Patch disable relocation failed. Error: " + ex.Message );
                }
            }
        }
    }
}
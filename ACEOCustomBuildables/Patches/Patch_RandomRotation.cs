using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace ACEOCustomBuildables.Patches
{
    [HarmonyPatch(typeof(AttachedObjectPositioningHandler))]
    [HarmonyPatch("ExecuteObjectPositioningAdjustment")]
    static class Patch_RandomRotation
    {   
        public static bool Prefix(AttachedObjectPositioningHandler __instance)
        {
            // If user specifies in config for no random rotation.
            if (ACEOCustomBuildablesConfig.disableRandomRotation)
            {
                UnityEngine.Object.Destroy( __instance );
                return false;
            }

            // If the item is custom
            if (__instance.gameObject.transform.parent.parent.TryGetComponent(out CustomItemSerializableInfo mod))
            {
                if (mod.useRandomRotation)
                {
                    return true;
                }

                UnityEngine.Object.Destroy( __instance );
                return false;
            }

            // Otherwise, go ahead
            return true;
        }
    }
}
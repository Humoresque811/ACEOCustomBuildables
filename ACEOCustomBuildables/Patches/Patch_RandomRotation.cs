using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace ACEOCustomBuildables
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
            if (__instance.gameObject.transform.parent.parent.TryGetComponent(out CustomItemSerializableComponent mod))
            {
                try
                {
                    if (JSONManager.buildableMods[mod.index].useRandomRotation)
                    {
                        return true;
                    }

                    UnityEngine.Object.Destroy(__instance);
                    return false;
                }
                catch (Exception ex)
                {
                    // If it fails log and return true because its vanilla :|
                    ACEOCustomBuildables.Log("[Mod Error] Random Rotation Patch failed. Error: " + ex.Message );
                    return true;
                }
            }

            // Otherwise, go ahead
            return true;
        }
    }
}
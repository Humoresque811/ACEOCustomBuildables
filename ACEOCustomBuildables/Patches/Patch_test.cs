using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;

namespace ACEOCustomBuildables
{
    
    //[HarmonyPatch(typeof(OverlayHandler), "ToggleConstructionOverlay")]
    static class Patch_test
    {
        // This is a patch for various tests (generally for buggixes)! ----------------------------------------------------------------------------
        /*public static void Postfix(OverlayHandler __instance, bool status)
        {
            PlaceableItem item;
            __instance.transform.parent.TryGetComponent<PlaceableItem>(out item);
            if (item != null)
            {
                ACEOCustomBuildables.Log("test status is " + status.ToString() + " on item " + item.ObjectName);
                ACEOCustomBuildables.Log("1st traverse result is " + Traverse.Create(__instance).Field<SpriteRenderer>("constructionOverlaySprite").Value.enabled.ToString());
            }
        }*/
    }
}

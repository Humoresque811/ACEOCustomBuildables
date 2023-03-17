using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;

namespace ACEOCustomBuildables
{
    
    [HarmonyPatch(typeof(PlaceObjectSquareDrag), "PlaceObject")]
    static class Patch_test
    {
        // This is for testing flooring, coming in a future update! ----------------------------------------------------------------------------
        /*public static void Postfix(PlaceObjectSquareDrag __instance, GameObject currentObject)
        {
            ACEOCustomBuildables.Log("3 " + currentObject.name);
        }*/
    }
}

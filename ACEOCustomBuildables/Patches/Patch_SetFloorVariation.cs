using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(MergedTile))]
    [HarmonyPatch("SetTile")]
    static class Patch_SetFloorVariation
    {   
        public static void Prefix(MergedTile __instance, ITileable tile, out ITileable __state)
        {
            __state = tile;
        }

        public static void Postfix(MergedTile __instance, ITileable __state)
        {
            if (__state.Quality < FileManager.Instance.floorIndexAddative)
            {
                return;
            }

            __instance.spriteRenderer.tileMode = SpriteTileMode.Continuous;
        }
    }
}
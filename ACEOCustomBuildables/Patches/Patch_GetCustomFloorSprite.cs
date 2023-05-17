using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;

namespace ACEOCustomBuildables
{
    
    [HarmonyPatch(typeof(DataPlaceholderMaterials), "GetFloorSprite")]
    static class Patch_GetCustomFloorSprite
    {
        public static bool Prefix(DataPlaceholderMaterials __instance, ref Sprite __result, int index)
        {
            if (index < FileManager.Instance.floorIndexAddative)
            {
                return true;
            }

            if (index - FileManager.Instance.floorIndexAddative > FileManager.Instance.buildableTypes[typeof(FloorMod)].Item2.buildableMods.Count - 1)
            {
                return true;
            }

            TexturedBuildableMod buildableMod = FileManager.Instance.buildableTypes[typeof(FloorMod)].Item2.buildableMods[index - FileManager.Instance.floorIndexAddative];
            if (!FileManager.Instance.GetTextureSprite(buildableMod, out Sprite oSprite, 256))
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get texture sprite from index in floor patch...");
                return true;
            }

            oSprite.texture.wrapMode = TextureWrapMode.Repeat;
            __result = oSprite;
            return false;
        }
    }
}

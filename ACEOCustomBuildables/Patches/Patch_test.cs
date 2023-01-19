using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;

namespace ACEOCustomBuildables
{
    /*
    [HarmonyPatch(typeof(GridManager), "AddObjectToMainGrid")]
    static class Patch_test
    {
        public static void Postfix(ApplicationVersionLabelUI __instance, Vector3[] borders)
        {
            ACEOCustomBuildables.Log("[Our test] It ran with: ");
            for (int i = 0; i < borders.Length; i++)
            {
                ACEOCustomBuildables.Log(borders[i].ToString());
            }
            ACEOCustomBuildables.Log("out");
        }
    }*/
}

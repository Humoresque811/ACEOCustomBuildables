using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace ACEOCustomBuildables.Patches
{
    [HarmonyPatch(typeof(DataPlaceholderItems))]
    [HarmonyPatch("LoadStickers")]
    static class Patch_LoadJSONs
    {
        public static void Postfix(DataPlaceholderItems __instance)
        {
            if (TemplateManager.getAllTemplates())
            {
                ACEOCustomBuildables.Log("[Mod Nuetral] Started loading mod info!");
                JSONManager.importJSON();
                if (JSONManager.buildableMods.Count != 0)
                {
                    ItemManager.clearBuildables();
                    UIManager.clearUI();
                    ACEOCustomBuildables.loadMods();
                    ACEOCustomBuildables.buildableCreationProccessInProgress = true;
                }
            }
        }
    }
}
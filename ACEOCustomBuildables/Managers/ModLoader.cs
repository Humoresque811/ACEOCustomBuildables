using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using ACEOCustomBuildables;
using HarmonyLib;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(SaveLoadGameDataController))]
    static class NewGameModPostix
    {
        [HarmonyPatch("StartNewGame")]
        public static void Prefix(SaveLoadGameDataController __instance)
        {
            ModLoader.LoadMods();
        }
    }

    class ModLoader : MonoBehaviour
    {
        public static void LoadMods()
        {
            ACEOCustomBuildables.Log("[Mod Nuetral] Started loading mod info!");
            Singleton<SceneMessagePanelUI>.Instance.SetLoadingText("Creating Custom Buildables...", 5);
            if (!TemplateManager.GetAllTemplates())
            {
                ACEOCustomBuildables.Log("[Mod Error] Template manager did not get all templates. Aborted mod loading!");
                return;
            }

            // Clear out last load's mods!
            ItemManager.clearBuildables();
            UIManager.clearUI();

            JSONManager.importJSON(); // Before this workshop mods have allready been queued
            if (!JSONManager.CanCountinueLoading())
            {
                ACEOCustomBuildables.Log("[Mod Nuetral] No mods loaded. Mod loading finished.");
                return;
            }

            ItemManager.createBuildables();
            if (ItemManager.itemsFailed)
            {
                ACEOCustomBuildables.Log("[Mod Error] There was an error creating an item mod (see above). Mod loading will countinue.");
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Success] Ended creating buildables. Created " + ItemManager.buildableModItems.Count + " buildable item(s)");
            }

            UIManager.createUI();
            if (UIManager.UIFailed)
            {
                ACEOCustomBuildables.Log("[Mod Error] There was an error creating an UI button (see above). Mod loading will countinue.");
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Success] Ended creating UI. Created " + UIManager.newIcons.Count + " UI button(s)");
            }
        }
	}
}
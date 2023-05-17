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
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                BuildableClassHelper.GetBuildableSourceCreator(type, out IBuildableSourceCreator buildableSourceCreator);
                buildableSourceCreator.ClearBuildableMods(true); 
            }

            // Load the JSON files
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                BuildableClassHelper.GetBuildableSourceCreator(type, out IBuildableSourceCreator buildableSourceCreator);
                buildableSourceCreator.ImportMods();
            }

            // Create buildables
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                BuildableClassHelper.GetBuildableCreator(type, out IBuildableCreator buildableCreator);
                buildableCreator.ClearBuildables();
                buildableCreator.CreateBuildables();
                ACEOCustomBuildables.Log($"[Mod Success] {buildableCreator.GetType().Name} finished creating buildables, creating {buildableCreator.buildables.Count} buildable(s)");
            }

            UIManager.ClearUI();
            UIManager.CreateAllUI();
            if (UIManager.UIFailed)
            {
                ACEOCustomBuildables.Log("[Mod Error] There was an error creating an UI button (see above). Mod loading will countinue.");
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Success] Ended creating UI. Created " + (UIManager.floorIcons.Count + UIManager.itemIcons.Count) + " UI button(s)");
            }
        }
	}
}
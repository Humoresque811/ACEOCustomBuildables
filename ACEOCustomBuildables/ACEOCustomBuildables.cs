using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UModFramework.API;
using Newtonsoft.Json;
using HarmonyLib;

namespace ACEOCustomBuildables
{
    [UMFHarmony(19)] //Set this to the number of harmony patches in your mod.
    [UMFScript]
    class ACEOCustomBuildables : MonoBehaviour
    {
        internal static void Log(string text, bool clean = false)
        {
            using (UMFLog log = new UMFLog()) log.Log(text, clean);
        }

        internal static void SimpleLog(string message)
        {
            Log(message);
        }

        [UMFConfig]
        public static void LoadConfig()
        {
            ACEOCustomBuildablesConfig.Load();
        }

		void Awake()
		{
			Log("ACEOCustomBuildables v" + UMFMod.GetModVersion().ToString(), true);
		}

        void Start()
        {
            try
            {
                ACEOCustomBuildables.Log("[Mod Neutral] Setting up creators!");
                FileManager fileManager = this.gameObject.AddComponent<FileManager>();
                fileManager.SetUp();

                ItemModSourceCreator itemModSourceCreator = this.gameObject.AddComponent<ItemModSourceCreator>();
                itemModSourceCreator.SetUp();
                FloorModSourceCreator floorModSourceCreator = this.gameObject.AddComponent<FloorModSourceCreator>();
                floorModSourceCreator.SetUp();
                TileableSourceCreator tileableSourceCreator = this.gameObject.AddComponent<TileableSourceCreator>();
                tileableSourceCreator.SetUp();

                ItemCreator itemManager = this.gameObject.AddComponent<ItemCreator>();
                itemManager.SetUp();
                FloorCreator floorManager = this.gameObject.AddComponent<FloorCreator>();
                floorManager.SetUp();
                TileableCreator tileableCreator = this.gameObject.AddComponent<TileableCreator>();
                tileableCreator.SetUp();

                fileManager.SetUpBuildableTypes();
                fileManager.SetUpBasePaths();

                UIManager.SetUpVariations();
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to set up buildable creators! Error: " + ex.Message);
            }

            try
            {
                EnumManager.ValidateEnums(new Action<string>(SimpleLog));
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log($"[Mod Error] Error while validating enums. Error: {ex.Message}");
            }
        }
    }
}
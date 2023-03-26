using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UModFramework.API;
using Newtonsoft.Json;

namespace ACEOCustomBuildables
{
    [UMFHarmony(11)] //Set this to the number of harmony patches in your mod.
    [UMFScript]
    class ACEOCustomBuildables : MonoBehaviour
    {
        public static bool buildableCreationProccessInProgress = false;

        internal static void Log(string text, bool clean = false)
        {
            using (UMFLog log = new UMFLog()) log.Log(text, clean);
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
                string path = Path.Combine(Application.persistentDataPath, JSONManager.pathAddativeBase, JSONManager.pathAddativeItems);
                Utils.CreateFolderIfNotExist(path);
                ACEOCustomBuildables.Log("[Mod Success] Got/Created buildable folder");
                JSONManager.basePath = path;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get path. Error: " + ex.Message);
            }   
        }


        void Update()
        {
            if (buildableCreationProccessInProgress)
            {
                loadMods();
            }
        }

        public static void loadMods()
        {
            if (!ItemManager.itemsCreated)
            {
                ItemManager.createBuildables();
                if (!ItemManager.itemsFailed)
                {
                    ACEOCustomBuildables.Log("[Mod Success] Ended creating buildables. Created " + ItemManager.buildableModItems.Count + " buildable item(s)");
                }
            }

            if (!UIManager.UICreated)
            {
                UIManager.createUI();
                if (!UIManager.UIFailed)
                {
                    ACEOCustomBuildables.Log("[Mod Success] Ended creating UI. Created " + UIManager.newIcons.Count + " UI button(s)");
                }
            }

            if (UIManager.UICreated && ItemManager.itemsCreated)
            {
                buildableCreationProccessInProgress = false;
            }
        }
	}
}
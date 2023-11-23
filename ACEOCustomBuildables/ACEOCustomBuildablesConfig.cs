using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UModFramework.API;

namespace ACEOCustomBuildables
{
    public class ACEOCustomBuildablesConfig
    {
        private static readonly string configVersion = "1.2";
        public static string displayConfigVersion = "v1.2";

        //Add your config vars here.
        public static bool disableRandomRotation = false;

        internal static void Load()
        {
            ACEOCustomBuildables.Log("Loading settings.");
            try
            {
                using (UMFConfig cfg = new UMFConfig())
                {
                    string cfgVer = cfg.Read("ConfigVersion", new UMFConfigString());
                    if (cfgVer != string.Empty && cfgVer != configVersion)
                    {
                        cfg.DeleteConfig(false);
                        ACEOCustomBuildables.Log("The config file was outdated and has been deleted. A new config will be generated.");
                    }

                    //cfg.Write("SupportsHotLoading", new UMFConfigBool(false)); //Uncomment if your mod can't be loaded once the game has started.
                    cfg.Write("ModDependencies", new UMFConfigStringArray(new string[] { "" })); //A comma separated list of mod/library names that this mod requires to function. Format: SomeMod:1.50,SomeLibrary:0.60
                    cfg.Read("LoadPriority", new UMFConfigString("Normal"));
                    cfg.Write("MinVersion", new UMFConfigString("0.53.6"));
                    cfg.Write("MaxVersion", new UMFConfigString("0.54.99999.99999")); //This will prevent the mod from being loaded after the next major UMF release
                    cfg.Write("UpdateURL", new UMFConfigString(""));
                    cfg.Write("ConfigVersion", new UMFConfigString(configVersion));
                    cfg.Write("UpdateURL", new UMFConfigString("https://umodframework.com/updatemod?id=36"));

                    ACEOCustomBuildables.Log("Finished UMF Settings.");

                    //Add your settings here
                    disableRandomRotation = cfg.Read("Disable Random Rotation", new UMFConfigBool(false, false, false), "Disables the random sprite rotation of plants and such.");

                    ACEOCustomBuildables.Log("Finished loading settings.");
                }
            }
            catch (Exception e)
            {
                ACEOCustomBuildables.Log("Error loading mod settings: " + e.Message + "(" + e.InnerException?.Message + ")");
            }
        }
    }
}
using FSG.MeshAnimator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ACEOCustomBuildables
{
    class FileManager : MonoBehaviour
    {
        public static FileManager Instance { get; private set; }

        public Dictionary<Type, Tuple<string, IBuildableSourceCreator, IBuildableCreator>> buildableTypes { get; private set; }

        public string basePath { get; private set; }

        public readonly string pathAddativeBase = "Buildables";
        public readonly int floorIndexAddative = 100;
        public readonly int tileableIndexAddative = 100;

        public void SetUp()
        {
            ACEOCustomBuildables.Log("[Mod Init] FileManager is being setup!");
            Instance = this;
        }

        public void SetUpBuildableTypes()
        {
            ACEOCustomBuildables.Log("[Mod Init] Setting up Custom Buidables types!");
            this.buildableTypes = new Dictionary<Type, Tuple<string, IBuildableSourceCreator, IBuildableCreator>>();

            Tuple<string, IBuildableSourceCreator, IBuildableCreator> itemTuple = new Tuple<string, IBuildableSourceCreator, IBuildableCreator>("Items", ItemModSourceCreator.Instance, ItemCreator.Instance);
            buildableTypes.Add(typeof(ItemMod), itemTuple);

            Tuple<string, IBuildableSourceCreator, IBuildableCreator> floorTuple = new Tuple<string, IBuildableSourceCreator, IBuildableCreator>("Floors", FloorModSourceCreator.Instance, FloorCreator.Instance);
            buildableTypes.Add(typeof(FloorMod), floorTuple);

            Tuple<string, IBuildableSourceCreator, IBuildableCreator> tileableTuple = new Tuple<string, IBuildableSourceCreator, IBuildableCreator>("Tileables", TileableSourceCreator.Instance, TileableCreator.Instance);
            buildableTypes.Add(typeof(TileableMod), tileableTuple);
        }

        public void SetUpBasePaths()
        {
            ACEOCustomBuildables.Log("[Mod Init] Starting basepath initilization!");
            try
            {
                string basePath = Path.Combine(Application.persistentDataPath, pathAddativeBase);
                if (!Directory.Exists(basePath))
                {
                    ACEOCustomBuildables.Log("[Mod Neutral] Basepath does not exist. Creating folder!");
                    Utils.CreateFolderIfNotExist(basePath);
                }
                this.basePath = basePath;

                foreach (Type type in buildableTypes.Keys.ToArray())
                {
                    string extendedPath = Path.Combine(basePath, buildableTypes[type].Item1);
                    if (!Directory.Exists(extendedPath))
                    {
                        ACEOCustomBuildables.Log($"[Mod Neutral] The folder for buildable type \"{buildableTypes[type].Item1}\" does not exist. Creating folder!");
                        Utils.CreateFolderIfNotExist(extendedPath);
                    }
                }

                ACEOCustomBuildables.Log("[Mod Success] Got/Created buildable folders");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Error creating basepaths! Error: " + ex.Message);
            }
        }

        public bool GetTextureSprite(TexturedBuildableMod buildableMod, out Sprite sprite, int pixelsPerUnit = 256)
        {
            bool result = GetTextureSpriteFromPath(buildableMod, "Texture", out Sprite outSprite, pixelsPerUnit);
            sprite = outSprite;
            return result;
        }

        public bool GetShadowSprite(TexturedBuildableMod buildableMod, out Sprite sprite, int pixelsPerUnit = 256)
        {
            bool result = GetTextureSpriteFromPath(buildableMod, "Shadow", out Sprite outSprite, pixelsPerUnit);
            sprite = outSprite;
            return result;
        }

        public bool GetIconSprite(TexturedBuildableMod buildableMod, out Sprite sprite, int pixelsPerUnit = 256)
        {
            bool result = GetTextureSpriteFromPath(buildableMod, "Icon", out Sprite outSprite, pixelsPerUnit);
            sprite = outSprite;
            return result;
        }


        private bool GetTextureSpriteFromPath(TexturedBuildableMod buildableMod, string spriteType, out Sprite sprite, int pixelsPerUnit = 256)
        {
            string path;
            string pathAddative = "";
            if (string.IsNullOrEmpty(buildableMod.pathToUse))
            {
                foreach (Type type in this.buildableTypes.Keys)
                {
                    if (Type.Equals(type, buildableMod.GetType()))
                    {
                        pathAddative = this.buildableTypes[type].Item1;
                    }
                }
                if (String.IsNullOrEmpty(pathAddative))
                {
                    ACEOCustomBuildables.Log($"[Mod Error] Buildable mod to get sprite for is not a valid type (being {buildableMod.GetType()}).");
                    sprite = null;
                    return false;
                }

                path = Path.Combine(basePath, pathAddative) + Path.DirectorySeparatorChar;
            }
            else
            {
                path = buildableMod.pathToUse + Path.DirectorySeparatorChar;
            }

            // Determine which texture to use
            if (spriteType == "Texture")
            {
                path += buildableMod.texturePath;
            }
            else if (string.Equals(spriteType, "Shadow"))
            {
                if (!(buildableMod is ShadowedBuildableMod))
                {
                    sprite = Singleton<DataPlaceholderItems>.Instance.smallPlantIcon;
                    return false;
                }

                ShadowedBuildableMod shadowedMod = buildableMod as ShadowedBuildableMod;

                if (string.Equals(shadowedMod.shadowPath, "autogenerate"))
                {
                    path += shadowedMod.texturePath;
                }
                else
                {
                    path += shadowedMod.shadowPath;
                }
            }
            else if (spriteType == "Icon")
            {
                if (string.Equals(buildableMod.iconPath, "autogenerate"))
                {
                    path += buildableMod.texturePath;
                }
                else
                {
                    path += buildableMod.iconPath;
                }
            }
            else
            {
                // Return the Fallback image
                ACEOCustomBuildables.Log("[Mod Error] Invalid image type input into the image loader.");
                sprite = Singleton<DataPlaceholderItems>.Instance.smallPlantIcon;
                return false;
            }

            // If texture keyword correct, then check if it exists
            if (File.Exists(path))
            {
                // This should be whats returned
                sprite = Utils.LoadImage(path, pixelsPerUnit, true);
                return true;
            }

            // If it doesn't exist, return this
            sprite = Singleton<DataPlaceholderItems>.Instance.smallPlantIcon;
            return false;
        }

        public bool GetTexture2DFromPath(TexturedBuildableMod buildableMod, out Texture2D texture2D)
        {
            string path;
            string pathAddative = "";

            if (string.IsNullOrEmpty(buildableMod.pathToUse))
            {
                foreach (Type type in this.buildableTypes.Keys)
                {
                    if (Type.Equals(type, buildableMod.GetType()))
                    {
                        pathAddative = this.buildableTypes[type].Item1;
                    }
                }
                if (String.IsNullOrEmpty(pathAddative))
                {
                    ACEOCustomBuildables.Log($"[Mod Error] Buildable mod to get texture2D for is not a valid type (being {buildableMod.GetType()}).");
                    texture2D = null;
                    return false;
                }

                path = Path.Combine(basePath, pathAddative) + Path.DirectorySeparatorChar;
            }
            else
            {
                path = buildableMod.pathToUse + Path.DirectorySeparatorChar;
            }

            path += buildableMod.texturePath;

            // If texture keyword correct, then check if it exists
            if (File.Exists(path))
            {
                // This should be whats returned
			    byte[] data = File.ReadAllBytes(path);
			    texture2D = new Texture2D(2, 2)
			    {
				    filterMode = FilterMode.Bilinear
			    };
			    texture2D.LoadImage(data);
			    if (GameSettingManager.CompressImages)
			    {
				    texture2D.Compress(true);
			    }
                texture2D.Apply(true, true);
		        return true;
            }

            // If it doesn't exist, return this
            texture2D = null;
            return false;
        }
        
        public string GetJSONFileContent(string path)
        {
            if (!File.Exists(path))
            {
                ACEOCustomBuildables.Log("[Mod Error] Nothing at GetJSONFileContent's provided search path!");
                return null;
            }

            // We know the file exists now!
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        public bool GetCorrectPath(ref string path, Type modtype, Action<string> logger)
        {
            if (string.IsNullOrEmpty(path))
            {
                BuildableClassHelper.GetBuildablePathExtension(modtype, out string pathExtension);
                path = Path.Combine(basePath, pathExtension);
                logger.Invoke("[Mod Neutral] Started reading the LocalLow buildables folder");
                return false;
            }
            else
            {
                logger.Invoke("[Mod Neutral] Started reading a folder (Workshop or Native mod folder)");
                return true;
            }
        }

        public bool GetDirectories(string path, out string[] directories, Action<string> logger)
        {
            try
            {
                directories = Directory.GetFiles(path, "*.json");
                return true;
            }
            catch (Exception ex)
            {
                logger.Invoke("[Mod Error] Couldn't get JSON file paths... Error: " + ex.ToString());
                directories = null;
                return false;
            }
        }

        public void CrossCheckIds(Action<string> logger)
        {
            foreach (Type type in buildableTypes.Keys)
            {
                List<TexturedBuildableMod> buildableMods = buildableTypes[type].Item2.buildableMods; // The buildables we're checking
                List<TexturedBuildableMod> buildableMods2 = new List<TexturedBuildableMod>(); // All others

                foreach (Type type2 in buildableTypes.Keys)
                {
                    if (Type.Equals(type, type2))
                    {
                        continue;
                    }

                    buildableMods2.AddRange(buildableTypes[type2].Item2.buildableMods);
                }

                if (buildableMods == null || buildableMods.Count == 0 || buildableMods2 == null || buildableMods2.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < buildableMods.Count; i++)
                {

                    if (!buildableMods2.Contains(buildableMods[i]))
                    {
                        continue;
                    }

                    logger.Invoke($"[Buildable Error] You have two mods with the same id... This will result in problems with saveload, so the mod " +
                        $"called {BuildableClassHelper.GetModIdentification(buildableMods[i])} was removed.");
                    DialogPanel.Instance.ShowMessagePanel($"[Airport CEO Custom Buildables] You have two mods with the same id.. This will result in problems with saveload, so the mod " +
                        $"called {BuildableClassHelper.GetModIdentification(buildableMods[i])} was removed", Color.black);
                    buildableTypes[type].Item2.buildableMods.RemoveAt(i);
                    continue;
                }
            }
        }
    }
}

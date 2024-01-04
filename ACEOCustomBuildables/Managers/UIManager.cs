using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Epic.OnlineServices.AntiCheatServer;

namespace ACEOCustomBuildables
{
    class UIManager : MonoBehaviour
    {
        public static Dictionary<Type, List<int[]>> variations = new Dictionary<Type, List<int[]>>();

        public static List<GameObject> floorIcons = new List<GameObject>();
        public static List<GameObject> itemIcons = new List<GameObject>();
        public static List<GameObject> tileableIcons = new List<GameObject>();
        public static bool UIFailed = false;

        public static void SetUpVariations()
        {
            variations = new Dictionary<Type, List<int[]>>();

            foreach (Type modType in FileManager.Instance.buildableTypes.Keys.ToArray())
            {
                if (!typeof(IVariationMod).IsAssignableFrom(modType))
                {
                    continue;
                }

                variations.Add(modType, new List<int[]>());
            }
        }
        
        public static void ClearUI()
        {
            floorIcons.DestroyResetList();
            itemIcons.DestroyResetList();
            tileableIcons.DestroyResetList();

            variations = new Dictionary<Type, List<int[]>>();
        }

        public static void CreateAllUI()
        {
            foreach (Type variationModType in variations.Keys.ToArray())
            {
                if (!typeof(IVariationMod).IsAssignableFrom(variationModType))
                {
                    continue;
                }

                BuildableClassHelper.GetBuildableSourceCreator(variationModType, out IBuildableSourceCreator buildableSourceCreator);

                if (buildableSourceCreator.buildableMods.Count <= 0)
                {
                    continue;
                }

                IVariationMod[] variationMods = new IVariationMod[buildableSourceCreator.buildableMods.Count];
                for (int i = 0; i < buildableSourceCreator.buildableMods.Count; i++)
                {
                    variationMods[i] = (IVariationMod)buildableSourceCreator.buildableMods[i];
                }

                variations[variationModType] = GetVariationList(variationMods);
            }

            foreach (Type modType in FileManager.Instance.buildableTypes.Keys)
            {
                List<TexturedBuildableMod> buildableMods = FileManager.Instance.buildableTypes[modType].Item2.buildableMods;
                List<GameObject> buildables = FileManager.Instance.buildableTypes[modType].Item3.buildables;

                if (buildableMods.Count != buildables.Count)
                {
                    ACEOCustomBuildables.Log($"[Mod Error] There are not the same amount of buildable mods as buildables in type {modType.Name}. Skipped loading");
                    continue;
                }

                Action<Type, TexturedBuildableMod, GameObject, int> createUIIcon = null;
                if (BuildableClassHelper.IsValidVariationModType(modType))
                {
                    createUIIcon = CreateUIIconVariation;
                }
                else
                {
                    createUIIcon = CreateUIIcon;
                }

                for (int i = 0; i < buildableMods.Count; i++)
                {
                    createUIIcon(modType, buildableMods[i], buildables[i], i);
                }
            }
        }

        private static void CreateUIIconVariation(Type modType, TexturedBuildableMod buildableMod, GameObject buildable, int index)
        {
            if (ShouldCreateIcon(index, variations[modType]))
            {
                CreateUI(buildableMod, buildable, modType); 
            }
            else
            {
                floorIcons.Add(null);
            }
        }

        private static void CreateUIIcon(Type modType, TexturedBuildableMod buildableMod, GameObject buildable, int index)
        {
            CreateUI(buildableMod, buildable, modType);
        }

        private static void CreateUI(TexturedBuildableMod buildableMod, GameObject buildable, Type modType)
        {
            try
            {
                // Get template
                GameObject panel = TemplateManager.UIPanels[buildableMod.buildMenu];
                    
                // UI Creation
                GameObject button = TemplateManager.UIPanels["DecorationViewport"].transform.GetChild(0).gameObject;
                GameObject newButton = Instantiate(button, Vector3.zero, button.transform.rotation);
                newButton.name = $"CustomItem";
                newButton.transform.SetParent(panel.transform);

                FileManager.Instance.GetIconSprite(buildableMod, out Sprite sprite);
                newButton.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

                Destroy(newButton.GetComponent<BuildButtonAssigner>());
                Destroy(newButton.GetComponent<BuildButtonTextManager>());

                newButton.gameObject.AddComponent(typeof(CustomBuildUI));
                CustomBuildUI buildUI = newButton.GetComponent<CustomBuildUI>();

                buildUI.buildableName = buildableMod.name;
                buildUI.buildableDescription = buildableMod.description;

                ItemMod itemMod = buildableMod as ItemMod;
                FloorMod floorMod = buildableMod as FloorMod;
                if (itemMod != null)
                {
                    buildUI.buildableCost = itemMod.buildCost;
                    buildUI.buildableOperatingCost = itemMod.operationCost;
                }
                else if (floorMod != null)
                {
                    buildUI.buildableCost = 2;
                    buildUI.buildableOperatingCost = 0;
                }

                buildUI.assignedButton = newButton.GetComponent<Button>();
                buildUI.assignedObject = buildable;
                buildUI.modType = modType;
                buildUI.assignedAnimator = newButton.GetComponent<Animator>();

                buildUI.convertButtonToCustom();

                if (modType == typeof(ItemMod))
                {
                    itemIcons.Add(newButton);
                }
                else if (modType == typeof(FloorMod))
                {
                    floorIcons.Add(newButton);
                }
                else if (modType == typeof(TileableMod))
                {
                    tileableIcons.Add(newButton);
                }
                else
                {
                    ACEOCustomBuildables.Log("[Mod Error] UI icon creators type not an expected type!");
                }
                ACEOCustomBuildables.Log("[Mod Success] Created UI button \"" + buildableMod.name + "\" successfully");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Creating UI button \"" + buildableMod.name + "\" failed. Error: " + ex.Message);
            }

            UIFailed = false;
        }

        public static bool IndexHasVariations(Type modType, int index)
        {
            if (!BuildableClassHelper.IsValidVariationModType(modType))
            {
                ACEOCustomBuildables.Log($"[Mod Error] Invalid mod type, in {MethodBase.GetCurrentMethod().Name} with type being {modType.Name}");
                return false;
            }

            if (variations[modType][index].Length != 1)
            {
                return true;
            }

            if (variations[modType][index][0] == -1)
            {
                return false;
            }

            return true;
        }

        public static int[] GetAllVariations(Type modType, int index)
        {
            if (!BuildableClassHelper.IsValidVariationModType(modType))
            {
                ACEOCustomBuildables.Log($"[Mod Error] Invalid mod type, in {MethodBase.GetCurrentMethod().Name} with type being {modType.Name}");
                return new int[0];
            }

            List<int> ints = new List<int>
            {
                index
            };
            ints.AddRange(variations[modType][index]);
            return ints.ToArray();
        }

        private static List<int[]> GetVariationList(in IVariationMod[] floorMods)
        {
            List<int[]> linkedIDs = new List<int[]>();
            for (int i = 0; i < floorMods.Length; i++)
            {
                if (string.Equals(floorMods[i].variationID, "None") || string.IsNullOrEmpty(floorMods[i].variationID))
                {
                    linkedIDs.Add(new int[1] {-1});
                    continue;
                }

                List<int> ints = new List<int>();
                for (int k = 0; k < floorMods.Length; k++)
                {
                    if (i == k)
                    {
                        continue;
                    }

                    if (string.Equals(floorMods[i].variationID, floorMods[k].variationID))
                    {
                        ints.Add(k);
                    }
                }

                linkedIDs.Add(ints.ToArray());
            }

            return linkedIDs;
        }

        private static bool ShouldCreateIcon(int index, in List<int[]> otherVariations)
        {
            if (otherVariations[index].Length == 0)
            {
                return true;
            }

            if (otherVariations[index][0] == -1)
            {
                return true;
            }

            bool otherIconIsCreated = false;
            for (int i = 0; i < otherVariations[index].Length; i++)
            {
                if (floorIcons.Count < otherVariations[index][i])
                {
                    continue;
                }

                if (floorIcons[otherVariations[index][i]] != null)
                {
                    otherIconIsCreated = true;
                }
            }

            if (otherIconIsCreated)
            {
                return false;
            }
            return true;
        }
	}
}
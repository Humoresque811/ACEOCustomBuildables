using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class UIManager : MonoBehaviour
    {
        private static List<int[]> otherVariations = new List<int[]>();
        public static List<GameObject> floorIcons = new List<GameObject>();
        public static List<GameObject> itemIcons = new List<GameObject>();
        public static List<GameObject> tileableIcons = new List<GameObject>();
        public static bool UIFailed = false;

        public static void ClearUI()
        {
            for (int i = 0; i < floorIcons.Count; i++)
            {
                Destroy(floorIcons[i]);
            }
            floorIcons = new List<GameObject>();

            for (int i = 0; i < itemIcons.Count; i++)
            {
                Destroy(itemIcons[i]);
            }
            itemIcons = new List<GameObject>();

            for (int i = 0; i < tileableIcons.Count; i++)
            {
                Destroy(tileableIcons[i]);
            }
            tileableIcons = new List<GameObject>();

            otherVariations = new List<int[]>();
        }

        public static void CreateAllUI()
        {
            if (FloorModSourceCreator.Instance.buildableMods.Count > 0)
            {
                // This is for variation compiling
                List<FloorMod> floorMods = new List<FloorMod>();
                foreach (TexturedBuildableMod buildableMod in FloorModSourceCreator.Instance.buildableMods)
                {
                    FloorMod floorMod = buildableMod as FloorMod;
                    if (floorMod == null)
                    {
                        ACEOCustomBuildables.Log("[Mod Error] A buildable mod in FloorModSourceCreator is not a buildable mod!");
                        continue;
                    }

                    floorMods.Add(floorMod);
                }

                otherVariations = GetVariationList(floorMods);
            }


            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                List<TexturedBuildableMod> buildableMods = FileManager.Instance.buildableTypes[type].Item2.buildableMods;
                List<GameObject> buildables = FileManager.Instance.buildableTypes[type].Item3.buildables;

                if (buildableMods.Count != buildables.Count)
                {
                    ACEOCustomBuildables.Log($"[Mod Error] There are not the same amount of buildable mods as buildables in type {type.Name}. Skipped loading");
                    continue;
                }

                for (int i = 0; i < buildableMods.Count; i++)
                {
                    if (type != typeof(FloorMod))
                    {
                        CreateUI(buildableMods[i], buildables[i], type);
                        continue;

                    }

                    if (ShouldCreateIcon(i, otherVariations))
                    {
                       CreateUI(buildableMods[i], buildables[i], type); 
                    }
                    else
                    {
                        floorIcons.Add(null);
                    }
                }
            }
        }

        private static void CreateUI(TexturedBuildableMod buildableMod, GameObject buildable, Type type)
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
                if (itemMod != null)
                {
                    buildUI.buildableCost = itemMod.buildCost;
                    buildUI.buildableOperatingCost = itemMod.operationCost;
                }
                else
                {
                    buildUI.buildableCost = 2;
                    buildUI.buildableOperatingCost = 0;
                }

                buildUI.assignedButton = newButton.GetComponent<Button>();
                buildUI.assignedObject = buildable;
                buildUI.modType = type;
                buildUI.assignedAnimator = newButton.GetComponent<Animator>();

                buildUI.convertButtonToCustom();

                if (type == typeof(ItemMod))
                {
                    itemIcons.Add(newButton);
                }
                else if (type == typeof(FloorMod))
                {
                    floorIcons.Add(newButton);
                }
                else if (type == typeof(TileableMod))
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

        public static bool IndexHasVariations(int index)
        {
            if (otherVariations[index].Length != 1)
            {
                return true;
            }

            if (otherVariations[index][0] == -1)
            {
                return false;
            }

            return true;
        }

        public static int[] GetAllVariations(int index)
        {
            List<int> ints = new List<int>
            {
                index
            };
            ints.AddRange(otherVariations[index]);
            return ints.ToArray();
        }

        private static List<int[]> GetVariationList(in List<FloorMod> floorMods)
        {
            List<int[]> linkedIDs = new List<int[]>();
            for (int i = 0; i < floorMods.Count; i++)
            {
                if (string.Equals(floorMods[i].floorVariationId, "None") || string.IsNullOrEmpty(floorMods[i].floorVariationId))
                {
                    linkedIDs.Add(new int[1] {-1});
                    continue;
                }

                List<int> ints = new List<int>();
                for (int k = 0; k < floorMods.Count; k++)
                {
                    if (i == k)
                    {
                        continue;
                    }

                    if (string.Equals(floorMods[i].floorVariationId, floorMods[k].floorVariationId))
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
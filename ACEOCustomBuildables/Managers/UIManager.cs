using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class UIManager : MonoBehaviour
    {
        public static List<GameObject> newIcons = new List<GameObject>();
        public static bool UIFailed = false;

        public static void ClearUI()
        {
            for (int i = 0; i < newIcons.Count; i++)
            {
                Destroy(newIcons[i]);
            }
            newIcons = new List<GameObject>();
        }

        public static void CreateAllUI()
        {
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                List<TexturedBuildableMod> buildableMods = FileManager.Instance.buildableTypes[type].Item2.buildableMods;
                List<GameObject> buildables = FileManager.Instance.buildableTypes[type].Item3.buildables;

                if (buildableMods.Count != buildables.Count)
                {
                    ACEOCustomBuildables.Log($"[Mod Error] There are not the same amount of buildable mods as buildables in type {type.Name}.");
                    continue;
                }

                for (int i = 0; i < buildableMods.Count; i++)
                {

                    /*if (buildables[i].TryGetComponent<PlaceableFloor>(out PlaceableFloor placeableFloor))
                    {
                        ACEOCustomBuildables.Log("has + " + placeableFloor.variationIndex.ToString());
                        continue;
                    }
                    else
                    {
                        ACEOCustomBuildables.Log("doesnt");
                    }*/

                    CreateUI(buildableMods[i], buildables[i], type);
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
                buildUI.buildableCost = buildableMod.buildCost;
                buildUI.buildableOperatingCost = buildableMod.operationCost;

                buildUI.assignedButton = newButton.GetComponent<Button>();
                buildUI.assignedObject = buildable;
                buildUI.modType = type;
                buildUI.assignedAnimator = newButton.GetComponent<Animator>();

                buildUI.convertButtonToCustom();

                newIcons.Add(newButton);
                ACEOCustomBuildables.Log("[Mod Success] Created UI button \"" + buildableMod.name + "\" successfully");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Creating UI button \"" + buildableMod.name + "\" failed. Error: " + ex.Message);
            }

            UIFailed = false;
        }
	}
}
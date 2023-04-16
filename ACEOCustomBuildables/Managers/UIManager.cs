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
        public static bool UICreated = false;
        public static bool UIFailed = false;

        public static void clearUI()
        {
            for (int i = 0; i < newIcons.Count; i++)
            {
                Destroy(newIcons[i]);
            }
            newIcons = new List<GameObject>();
            UICreated = false;
        }

        public static void createUI()
        {
            for (int i = 0; i < JSONManager.itemMods.Count; i++)
            {
                try
                {
                    // Get template
                    GameObject panel = TemplateManager.UIPanels[JSONManager.itemMods[i].buildMenu];
                    
                    // UI Creation
                    GameObject button = TemplateManager.UIPanels["DecorationViewport"].transform.GetChild(0).gameObject;
                    GameObject newButton = Instantiate(button, Vector3.zero, button.transform.rotation);
                    newButton.name = $"CustomItem {i}";
                    newButton.transform.SetParent(panel.transform);
                    newButton.transform.GetChild(0).GetComponent<Image>().sprite = JSONManager.getSpriteFromPath(i, "Icon");
                    Destroy(newButton.GetComponent<BuildButtonAssigner>());
                    Destroy(newButton.GetComponent<BuildButtonTextManager>());

                    newButton.gameObject.AddComponent(typeof(CustomBuildUI));
                    CustomBuildUI buildUI = newButton.GetComponent<CustomBuildUI>();

                    buildUI.buildableName = JSONManager.itemMods[i].name;
                    buildUI.buildableDescription = JSONManager.itemMods[i].description;
                    buildUI.buildableCost = JSONManager.itemMods[i].buildCost;
                    buildUI.buildableOperatingCost = JSONManager.itemMods[i].operationCost;

                    buildUI.assignedButton = newButton.GetComponent<Button>();
                    buildUI.assignedObject = ItemManager.buildableModItems[i].gameObject;
                    //buildUI.assignedObject = Singleton<BuildingController>.Instance.terminalPrefabs.hardFloor; // TESTING CODE ----------------------------------------------------------------------------
                    buildUI.assignedAnimator = newButton.GetComponent<Animator>();

                    buildUI.convertButtonToCustom();

                    newIcons.Add(newButton);
                    ACEOCustomBuildables.Log("[Mod Success] Created UI button \"" + JSONManager.itemMods[i].name + "\" successfully");
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Creating UI button \"" + JSONManager.itemMods[i].name + "\" failed. Error: " + ex.Message);
                }
            }

            UIFailed = false;
            UICreated = true;
        }
	}
}
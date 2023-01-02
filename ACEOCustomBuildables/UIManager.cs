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
                newIcons.RemoveAt(i);
            }
            UICreated = false;
        }

        public static void createUI()
        {
            for (int i = 0; i < JSONManager.buildableMods.Count; i++)
            {
                try
                {
                    // Get template
                    GameObject panel = TemplateManager.UITemplate;

                    // UI Creation
                    GameObject button = panel.transform.GetChild(2).GetChild(1).gameObject;
                    GameObject newButton = Instantiate(button, Vector3.zero, button.transform.rotation);
                    newButton.name = "CustomItem " + i;
                    newButton.transform.SetParent(panel.transform.GetChild(2));
                    newButton.transform.GetChild(0).GetComponent<Image>().sprite = JSONManager.getSpriteFromPath(i, "Icon");
                    Destroy(newButton.GetComponent<BuildButtonAssigner>());
                    Destroy(newButton.GetComponent<BuildButtonTextManager>());

                    newButton.gameObject.AddComponent(typeof(CustomBuildUI));
                    CustomBuildUI buildUI = newButton.GetComponent<CustomBuildUI>();

                    buildUI.buildableName = JSONManager.buildableMods[i].name;
                    buildUI.buildableDescription = JSONManager.buildableMods[i].description;
                    buildUI.buildableCost = JSONManager.buildableMods[i].buildCost;
                    buildUI.buildableOperatingCost = JSONManager.buildableMods[i].operationCost;

                    buildUI.assignedButton = newButton.GetComponent<Button>();
                    buildUI.assignedObject = ItemManager.buildableModItems[i].gameObject;
                    buildUI.assignedAnimator = newButton.GetComponent<Animator>();

                    buildUI.convertButtonToCustom();

                    newIcons.Add(newButton);
                    ACEOCustomBuildables.Log("[Mod Success] Created UI button \"" + JSONManager.buildableMods[i].name + "\" successfully");
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Creating UI button \"" + JSONManager.buildableMods[i].name + "\" failed. Error: " + ex.Message);
                }
            }

            UIFailed = false;
            UICreated = true;
        }
	}
}
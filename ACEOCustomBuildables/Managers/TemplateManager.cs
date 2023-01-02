using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class TemplateManager : MonoBehaviour
    {
        // Templates
        public static GameObject UITemplate;
        public static GameObject ItemTemplate;
        public static ObjectPlacementController objectPlacementControllerTemplate; 


        // Main function to be called
        /// <summary>
        /// Gets all required templates to start creating items!
        /// </summary>
        /// <returns>Returns true if all templates have been found, false if not</returns>
        public static bool getAllTemplates()
        {
            tryGetUITemplate();
            tryGetItemTemplate();
            tryGetPlacementControllerTemplate();

            if (UITemplate != null && ItemTemplate != null && objectPlacementControllerTemplate != null)
            {
                return true;
            }
            return false;
        }

        private static void tryGetUITemplate()
        {
            try
            {
                List<Transform> panels = Singleton<PlaceablePanelUI>.Instance.availablePanels;
                for (int i = 0; i < panels.Count; i++)
                {
                    if (panels[i].name == "DecorationViewport")
                    {
                        ACEOCustomBuildables.Log("[Mod Success] Got UI panel: " + panels[i].GetChild(0).GetChild(0).name);
                        UITemplate = panels[i].GetChild(0).GetChild(0).gameObject;
                    }
                }
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get UI panel. Error: " + ex.Message);
            }
        }

        private static void tryGetItemTemplate()
        {
            try
            {
                ItemTemplate = Singleton<BuildingController>.Instance.smallPlant;
                ACEOCustomBuildables.Log("[Mod Success] Got template item");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get template item. Error: " + ex.Message);
            }
        }

        private static void tryGetPlacementControllerTemplate()
        {
            try
            {
                objectPlacementControllerTemplate = Singleton<BuildingController>.Instance.GetComponent<ObjectPlacementController>();
                ACEOCustomBuildables.Log("[Mod Success] Got placement controller");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get placement controller. Error: " + ex.Message);
            }
        }

	}
}
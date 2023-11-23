using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace ACEOCustomBuildables
{
    class TemplateManager : MonoBehaviour
    {
        // Templates
        public static Dictionary<string, GameObject> UIPanels { get; private set; }
        public static GameObject ItemTemplate;
        public static GameObject FloorTemplate;
        public static GameObject TileableTemplate;
        public static ObjectPlacementController objectPlacementControllerTemplate; 


        // Main function to be called
        /// <summary>
        /// Gets all required templates to start creating items!
        /// </summary>
        /// <returns>Returns true if all templates have been found, false if not</returns>
        public static bool GetAllTemplates()
        {
            bool successFlag = true;
            if (!TryGetUIPanels())
            {
                successFlag = false;
            }
            if (!TryGetItemTemplate())
            {
                successFlag = false;
            }
            if (!TryGetFloorTemplate())
            {
                successFlag = false;
            }
            if (!TryGetTileableTemplate())
            {
                successFlag = false;
            }
            if (!TryGetPlacementControllerTemplate())
            {
                successFlag = false;
            }
            return successFlag;
        }

        private static bool TryGetUIPanels()
        {
            try
            {
                UIPanels = new Dictionary<string, GameObject>();
                List<Transform> panels = Singleton<PlaceablePanelUI>.Instance.availablePanels;

                for (int i = 0; i < panels.Count; i++)
                {
                    switch (panels[i].name)
                    {
                        case "ZoneAndRoomViewport":
                            UIPanels.Add("ZoneAndRoomViewport", panels[i].GetChild(0).GetChild(2).GetChild(0).gameObject);
                            break;
                        case "ConveyorBeltSystemViewport":
                            UIPanels.Add("ConveyorBeltSystemViewport", panels[i].GetChild(0).GetChild(3).GetChild(0).gameObject);
                            break;
                        case "StaffViewport":
                            UIPanels.Add("StaffViewport", panels[i].GetChild(0).GetChild(2).GetChild(0).gameObject);
                            break;
                        case "DeskViewport":
                            UIPanels.Add("DeskViewport", panels[i].GetChild(0).GetChild(2).GetChild(0).gameObject);
                            break;
                        case "SecurityViewport":
                            UIPanels.Add("SecurityViewport", panels[i].GetChild(0).GetChild(2).GetChild(0).gameObject);
                            break;
                        case "BathroomViewport":
                            UIPanels.Add("BathroomViewport", panels[i].GetChild(0).GetChild(1).GetChild(1).gameObject);
                            break;
                        case "ShopRoomViewport":
                            UIPanels.Add("ShopRoomViewport", panels[i].GetChild(0).GetChild(1).GetChild(0).gameObject);
                            break;
                        case "FoodRoomViewport":
                            UIPanels.Add("FoodRoomViewport", panels[i].GetChild(0).GetChild(1).GetChild(1).gameObject);
                            break;
                        case "AirlineLoungeViewport":
                            UIPanels.Add("AirlineLoungeViewport", panels[i].GetChild(0).GetChild(1).GetChild(0).gameObject);
                            break;
                        case "DecorationViewport":
                            UIPanels.Add("DecorationViewport", panels[i].GetChild(0).GetChild(0).GetChild(2).gameObject);
                            break;
                    }
                }

                ACEOCustomBuildables.Log("[Mod Success] Got UI panels!");
                return true;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get UI panel. Error: " + ex.Message);
                return false;
            }
        }

        private static bool TryGetItemTemplate()
        {
            try
            {
                ItemTemplate = Singleton<BuildingController>.Instance.smallPlant;
                ACEOCustomBuildables.Log("[Mod Success] Got item template");
                return true;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get item template. Error: " + ex.Message);
                return false;
            }
        }

        private static bool TryGetFloorTemplate()
        {
            try
            {
                FloorTemplate = Singleton<BuildingController>.Instance.terminalPrefabs.hardFloor;
                ACEOCustomBuildables.Log("[Mod Success] Got floor template");
                return true;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get floor template. Error: " + ex.Message);
                return false;
            }
        }

        private static bool TryGetTileableTemplate()
        {
            try
            {
                TileableTemplate = Singleton<BuildingController>.Instance.hedge;
                ACEOCustomBuildables.Log("[Mod Success] Got tileable template");
                return true;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get tileable template. Error: " + ex.Message);
                return false;
            }
        }

        private static bool TryGetPlacementControllerTemplate()
        {
            try
            {
                objectPlacementControllerTemplate = Singleton<BuildingController>.Instance.GetComponent<ObjectPlacementController>();
                ACEOCustomBuildables.Log("[Mod Success] Got placement controller");
                return true;
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to get placement controller. Error: " + ex.Message);
                return false;
            }
        }
	}
}
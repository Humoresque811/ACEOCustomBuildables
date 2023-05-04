using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace ACEOCustomBuildables
{
    class CustomBuildingController : MonoBehaviour
    {
        public static void spawnItem(GameObject item, Type type)
        {
            // Get template
            ObjectPlacementController placementController = TemplateManager.objectPlacementControllerTemplate;

            if (item != null && placementController != null)
            {
                try
                {
                    if (!item.activeSelf)
                    {
                        item.SetActive(true);
                    }

                    if (Type.Equals(type, typeof(FloorMod)))
                    {
                        if (!item.TryGetComponent<PlaceableFloor>(out PlaceableFloor placeableFloor))
                        {
                            ACEOCustomBuildables.Log("[Mod Error] Failed to get placeableFloor component on item...?");
                            return;
                        }

                        //item.transform.GetChild(0).localScale = ItemCreator.Instance.calculateScale(item.transform.GetChild(0).gameObject, 1f, 1f);                        

                        //VariationsHandler.CurrentVariationSizeType = placeableFloor.objectSize;
                        //VariationsHandler.CurrentVariationQualityType = placeableFloor.objectQuality;
                        VariationsHandler.currentVariationIndex = placeableFloor.variationIndex;
                        placementController.SetObject(item, 0);
                        return;
                    }

                    placementController.SetObject(item, 0);
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Failed to set object! Error: " + ex.Message);
                }
                return;
            }
            ACEOCustomBuildables.Log("[Mod Error] Hmm, Humoresque failed to code correctly. The item was null or the placment controller was. Fix it!!!");
        }
	}
} 
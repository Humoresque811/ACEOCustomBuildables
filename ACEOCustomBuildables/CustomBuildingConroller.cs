using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace ACEOCustomBuildables
{
    class CustomBuildingController : MonoBehaviour
    {
        public static void spawnItem(GameObject item)
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
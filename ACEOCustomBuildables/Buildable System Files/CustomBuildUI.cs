using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

namespace ACEOCustomBuildables
{
    class CustomBuildUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Button assignedButton;
        public GameObject assignedObject;
        public Animator assignedAnimator;

        public string buildableName;
        public string buildableDescription;
        public int buildableCost;
        public int buildableOperatingCost;

        public void convertButtonToCustom()
        {
            // Make the button custom and remove all things on click, re-add the appropriate ones
            try
            {
                assignedButton.onClick.RemoveAllListeners();
                assignedButton.onClick.AddListener(delegate ()
                {
                    CustomBuildingController.spawnItem(assignedObject);
                    Singleton<AudioController>.Instance.PlayAudio(Enums.AudioClip.PointerClick, false, 1f, 1f, false);
                    Singleton<PlaceablePanelUI>.Instance.EnableDisableSearchFieldInput(false);
                    Singleton<ObjectDescriptionPanelUI>.Instance.HidePanel();
                    ObjectPlacementController.hasAttemptedBuild = false;
                    EventSystem.current.SetSelectedGameObject(null);
                });
                assignedAnimator = assignedButton.GetComponent<Animator>();
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Error in convert button custom. Error: " + ex.Message);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventdata)
        {
            Singleton<ObjectDescriptionPanelUI>.Instance.HidePanel();
            Singleton<VariationsHandler>.Instance.TogglePanel(false);
            try
            {
                Singleton<AudioController>.Instance.PlayAudio(Enums.AudioClip.PointerEnter, true, 1f, 1f, false);
                ObjectDescriptionPanelUI ObjectDescriptionPanel = Singleton<ObjectDescriptionPanelUI>.Instance;
                ObjectDescriptionPanel.ShowTemplatePanel(assignedButton.transform, buildableName, buildableDescription);
                ObjectDescriptionPanel.contractorCostText.text = $"{LocalizationManager.GetLocalizedValue("ObjectDescriptionPanelUI.cs.key.21")} {buildableCost}";
                ObjectDescriptionPanel.operatingCostText.text = $"{LocalizationManager.GetLocalizedValue("ObjectDescriptionPanelUI.cs.key.27")} {buildableOperatingCost}";
                ObjectDescriptionPanel.objectImageInstructionText.text = "No Preview Availible For Custom Buildables...";

                assignedAnimator.Play("BounceButton");
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] PointerEnter Custom Build UI Failed... Error: " + ex.Message);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            try
            {
                assignedAnimator.Play("BounceDown");
                Singleton<ObjectDescriptionPanelUI>.Instance.HidePanel();
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] PointerExit Custom Build UI Failed... Error: " + ex.Message);
            }
        }
	}
} 
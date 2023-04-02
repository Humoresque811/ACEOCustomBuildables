using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using static OnlineMapsBingMapsElevation;

namespace ACEOCustomBuildables
{
    class InputCheckerHelper : MonoBehaviour
    {
        private static string currentDialog = "";
        private static Action<string> currentLogger = null;
        // This will be a future project. It is meant to allow simple checking of new vars, but the current idea was flawed. It will be for later
        public static void CheckItemMod(itemMod itemMod, Action<string> Logger)
        {
            if (Logger == null)
            {
                ACEOCustomBuildables.Log("[Mod Error] Logger provided to input checker is null!");
                return;
            }

            currentLogger = Logger;
            
            if (itemMod == null)
            {
                currentLogger("[Mod Error] Item mod provided to input checker is null");
                return;
            }

            currentDialog = $"[Buildable Non-Critical Issue] Your mod called {GetItemModIdentification(itemMod)} seems to have";

            if (itemMod.bogusOverride)
            {
                CheckIntItemModAtrribute(ref itemMod.x, 1, -1, "a x-value");
                CheckIntItemModAtrribute(ref itemMod.y, 1, -1, "a y-value");
            }
            else
            {
                CheckIntItemModAtrribute(ref itemMod.x, 1, 16, "a x-value");
                CheckIntItemModAtrribute(ref itemMod.y, 1, 16, "a y-value");
            }

            CheckIntItemModAtrribute(ref itemMod.buildCost, 1, -1, "a build cost"); // ten million max
            CheckIntItemModAtrribute(ref itemMod.operationCost, 1, -1, "an operation cost"); // ten million max
            CheckIntItemModAtrribute(ref itemMod.constructionEnergy, 1, 10000, "a construction energy value");
            CheckIntItemModAtrribute(ref itemMod.contractors, 1, 100, "a contractors value");
            CheckFloatItemModAtrribute(ref itemMod.shadowDistance, -10, 10, "a shadow distance value");
            CheckFloatItemModAtrribute(ref itemMod.shadowTextureSizeMultiplier, 1, 5, "a shadow texture size multiplier");

        }

        // Attribute Forwarders

        private static void CheckIntItemModAtrribute(ref int subject, in int min, in int max, in string variableNameText)
        {
            string output = IntCheck(ref subject, min, max);
            if (!string.IsNullOrEmpty(output))
            {
                string fullLog = $"{currentDialog} {variableNameText} {output}";
                ShowDialog(currentLogger, fullLog);
                return;
            }
        }

        private static void CheckFloatItemModAtrribute(ref float subject, in float min, in float max, in string variableNameText)
        {
            string output = FloatCheck(ref subject, min, max);
            if (!string.IsNullOrEmpty(output))
            {
                string fullLog = $"{currentDialog} {variableNameText} that is {output}. Please check the mod log for more info!";
                ShowDialog(currentLogger, fullLog);
                return;
            }
        }

        // Actual Checkers

        private static string IntCheck(ref int subject, in int min, in int max)
        {
            float subjectFloat = subject;
            string dialog = FloatCheck(ref subjectFloat, min, max);

            subject = Mathf.FloorToInt(subjectFloat);
            return dialog;
        }

        private static string FloatCheck(ref float subject, in float min, in float max)
        {
            string gameDialog = "";
            if (subject < min)
            {
                gameDialog = $"below the minimum of {min}, being {subject}, so it was changed to {min}";
                subject = min;
                return gameDialog;
            }

            int dontEnforeMaxNumber = -1;
            if (max == dontEnforeMaxNumber)
            {
                return "";
            }

            if (subject > max)
            {
                gameDialog = $"above the maximum of {max}, being {subject}, so it was changed to {max}";
                subject = max;
                return gameDialog;
            }

            return "";
        }

        // Axuilary Code

        private static string GetItemModIdentification(itemMod itemMod)
        {
            return GetModIdentification(itemMod.name, itemMod.id);
        }

        private static string GetModIdentification(string name, string id)
        {
            return $"\"{name}\", with id \"{id}\",";
        }

        private static void ShowDialog(Action<string> Logger, string fullLog)
        {
            Logger(fullLog);
            DialogPanel.Instance.ShowMessagePanel(fullLog);
        }
	}
}
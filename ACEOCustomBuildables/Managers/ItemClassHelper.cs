using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Linq;

namespace ACEOCustomBuildables
{
    class ItemClassHelper : MonoBehaviour
    {
        public static readonly string[] itemPlacementAreaOptions = new string[] { "Both", "Inside", "Outside" };
        private static string currentDialog = "";
        private static Action<string> currentLogger = null;

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
                CheckIntModAttribute(ref itemMod.x, 1, -1, "a x-value");
                CheckIntModAttribute(ref itemMod.y, 1, -1, "a y-value");
            }
            else
            {
                CheckIntModAttribute(ref itemMod.x, 1, 16, "a x-value");
                CheckIntModAttribute(ref itemMod.y, 1, 16, "a y-value");
            }

            CheckIntModAttribute(ref itemMod.buildCost, 1, -1, "a build cost");
            CheckIntModAttribute(ref itemMod.operationCost, 1, -1, "an operation cost");
            CheckIntModAttribute(ref itemMod.constructionEnergy, 1, 10000, "a construction energy value");
            CheckIntModAttribute(ref itemMod.contractors, 1, 100, "a contractors value");
            CheckFloatModAttribute(ref itemMod.shadowDistance, -10, 10, "a shadow distance value");
            CheckFloatModAttribute(ref itemMod.shadowTextureSizeMultiplier, 1, 5, "a shadow texture size multiplier");

            CheckStringModAttribute(ref itemMod.itemPlacementArea, itemPlacementAreaOptions, "a itemPlacementArea value");
            CheckStringModAttribute(ref itemMod.buildMenu, TemplateManager.UIPanels.Keys.ToArray(), "a buildMenu value");
        }

        // Attribute Forwarders
        private static void CheckStringModAttribute(ref string subject, in string[] options, in string variableName)
        {
            if (options == null)
            {
                return;
            }
            if (options.Length < 1)
            {
                return;
            }
            
            if (options.Contains(subject))
            {
                // We're good, its a valid option
                return;
            }

            string fullLog = $"{currentDialog} {variableName} that is not one of the possible options ({String.Join(", ", options)}), being \"{subject}\". " +
                $"It has been changed to \"{options[0]}\"";
            subject = options[0];
            ShowDialog(currentLogger, fullLog);
        }

        private static void CheckIntModAttribute(ref int subject, in int min, in int max, in string variableName)
        {
            string output = IntCheck(ref subject, min, max);
            if (!string.IsNullOrEmpty(output))
            {
                string fullLog = $"{currentDialog} {variableName} that is {output}. Please check the mod log for more info!";
                ShowDialog(currentLogger, fullLog);
                return;
            }
        }

        private static void CheckFloatModAttribute(ref float subject, in float min, in float max, in string variableName)
        {
            string output = FloatCheck(ref subject, min, max);
            if (!string.IsNullOrEmpty(output))
            {
                string fullLog = $"{currentDialog} {variableName} that is {output}. Please check the mod log for more info!";
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
        public static string GetItemModIdentification(itemMod itemMod)
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
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    class InputCheckerHelper : MonoBehaviour
    {
        // This will be a future project. It is meant to allow simple checking of new vars, but the current idea was flawed. It will be for later
        /*public static string checkItemMod(itemMod itemMod)
        {

            return "";
        }

        private static string floatCheck(float input, float min, float max, string modName, string variableName, bool critical, int index)
        {
            string messageConfigurer = string.Empty;
            if (input < min)
            {
                messageConfigurer = "low";
                if (critical)
                { 
                    JSONManager.itemMods.RemoveAt(index); 
                }
                else
                {
                    JSONManager.itemMods[index];
                }
            }
            else if (input > max)
            {
                messageConfigurer = "high";
                if (critical)
                { 
                    JSONManager.itemMods.RemoveAt(index); 
                }
                else
                {
                    input = max;
                }
            }
            else
            {
                return "";
            }

            string message = "Your mod \"" + modName + "\" seems to have a too " + messageConfigurer + " of a " + variableName + " value. ";
            return "";
        }*/
	}
}
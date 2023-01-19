using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(ApplicationVersionLabelUI), "Awake")]
    static class Patch_AppVersionLabel
    {
        public static void Postfix(ApplicationVersionLabelUI __instance)
        {
            TMP_Text tMP = __instance.transform.GetComponent<TextMeshProUGUI>();
            string str = tMP.text;

            str = str + "ACEO Custom Buildables " + ACEOCustomBuildablesConfig.displayConfigVersion + "\n";
            tMP.text = str;
        }
    }
}

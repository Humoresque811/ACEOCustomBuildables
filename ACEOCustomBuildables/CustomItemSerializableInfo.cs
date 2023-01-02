using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using static ACEOCustomBuildables.JSONManager;


namespace ACEOCustomBuildables
{
    // This entire class is just an placeholder/limited feature class, will be expanded for save load (determines between mods and non-mods)
    class CustomItemSerializableInfo : MonoBehaviour
    {
        string modId;
        public bool useRandomRotation;

        public void setModInfoFromIndex(int index)
        {
            modId = JSONManager.buildableMods[index].id;
            useRandomRotation = JSONManager.buildableMods[index].useRandomRotation;
        }

        public void setModInfoFromClass(buildableMod theClass)
        {
            modId = theClass.id;
            useRandomRotation = theClass.useRandomRotation;
        }
	}
}
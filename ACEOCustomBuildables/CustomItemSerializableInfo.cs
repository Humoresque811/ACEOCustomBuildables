using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using static ACEOCustomBuildables.JSONManager;

namespace ACEOCustomBuildables
{
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
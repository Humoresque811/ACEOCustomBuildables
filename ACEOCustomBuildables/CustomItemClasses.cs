using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    // Component
    public class CustomItemSerializableComponent : MonoBehaviour
    {
        public int index;
    }

    // Save file
    public class CustomItemSerializable
    {
        public string modId;
        public float spriteRotation;
        public float itemRotation;
        public float[] postion = new float[3];
        public int floor;
        public string referenceID;
    }

    // The Wrapper
    public class CustomItemSerializableWrapper
    {
        public List<CustomItemSerializable> customItemSerializables;

        // Instantly set
        public CustomItemSerializableWrapper(List<CustomItemSerializable> customItemSerializables)
        {
            this.customItemSerializables = new List<CustomItemSerializable>();
            this.customItemSerializables = customItemSerializables;
        }

        // Set manually
        public CustomItemSerializableWrapper()
        {
            this.customItemSerializables = new List<CustomItemSerializable>();
        }
    }

    // Item mod
    [System.Serializable]
    public class itemMod
    {
        public bool enabled;
        public string name;
        public string id;
        public string description;
        public int x;
        public int y;
        public string texturePath;
        public string shadowPath;
        public string iconPath;
        public int buildCost;
        public int operationCost;
        public bool bogusOverride;
        public string itemPlacementArea;
        public bool useRandomRotation;
        public int constructionEnergy;
        public int contractors;
        public float shadowDistance;
        public float shadowTextureSizeMultiplier = 1.0f;
        public string pathToUse; //This is used and created by JSON Manager, not to be inputted
    }

    // Floor mod
    public class floorMod
    {
        public bool enabled;
        public string name;
        public string id;
        public string description;
        public string texturePath;
        public string shadowPath;
        public string iconPath;
    }
}
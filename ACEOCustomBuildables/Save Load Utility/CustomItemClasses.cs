using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACEOCustomBuildables
{
    public class CustomItemSerializableComponent : MonoBehaviour
    {
        public int index;
    }

    public class CustomItemSerializable
    {
        public string modId;
        public float spriteRotation;
        public float itemRotation;
        public float[] postion = new float[3];
        public int floor;
        public string referenceID;
    }

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

}
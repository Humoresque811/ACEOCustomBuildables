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
        public void Setup(int index, Type type)
        {
            if (Type.Equals(type, typeof(ItemMod)))
            {
                this.itemIndex = index;
                this.floorIndex = nullInt;
            }
            if (Type.Equals(type, typeof(FloorMod)))
            {
                this.itemIndex = nullInt;
                this.floorIndex = index;
            }
        }

        public readonly int nullInt = -1;
        public int itemIndex;
        public int floorIndex;
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
}
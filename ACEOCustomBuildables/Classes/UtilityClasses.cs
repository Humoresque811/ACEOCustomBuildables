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

    public class CustomFloorSerializable
    {
        public string modId;
        public float[] position = new float[3];
        public float[] size = new float[2];
        public string tileType;
        public int floor;
    }

    // The Wrapper
    public class CustomSerializableWrapper
    {
        public List<CustomItemSerializable> customItemSerializables;
        public List<CustomFloorSerializable> customFloorSerializables;

        // Instantly set
        public CustomSerializableWrapper(List<CustomItemSerializable> customItemSerializables, List<CustomFloorSerializable> customFloorSerializables)
        {
            this.customItemSerializables = new List<CustomItemSerializable>();
            this.customItemSerializables = customItemSerializables;
            this.customFloorSerializables = customFloorSerializables;
        }

        // Set manually
        public CustomSerializableWrapper()
        {
            this.customItemSerializables = new List<CustomItemSerializable>();
            this.customFloorSerializables = new List<CustomFloorSerializable>();
        }
    }
}
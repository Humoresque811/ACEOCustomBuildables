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
                this.tileableIndex = nullInt;
            }
            if (Type.Equals(type, typeof(FloorMod)))
            {
                this.itemIndex = nullInt;
                this.floorIndex = index;
                this.tileableIndex = nullInt;
            }
            if (Type.Equals(type, typeof(TileableMod)))
            {
                this.itemIndex = nullInt;
                this.floorIndex = nullInt;
                this.tileableIndex = index;
            }
        }

        public readonly int nullInt = -1;
        public int itemIndex;
        public int floorIndex;
        public int tileableIndex;
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

    public class CustomTileableSerializable
    {
        public string modId;
        public float[] position = new float[3];
        public int floor;
        public string referenceID;
    }

    // The Wrapper
    public class CustomSerializableWrapper
    {
        public List<CustomItemSerializable> customItemSerializables;
        public List<CustomFloorSerializable> customFloorSerializables;
        public List<CustomTileableSerializable> customTileableSerializables;

        // Instantly set
        public CustomSerializableWrapper(List<CustomItemSerializable> customItemSerializables, List<CustomFloorSerializable> customFloorSerializables, List<CustomTileableSerializable> customTileableSerializables)
        {
            this.customItemSerializables = new List<CustomItemSerializable>();
            this.customItemSerializables = customItemSerializables;
            this.customFloorSerializables = customFloorSerializables;
            this.customTileableSerializables = customTileableSerializables;
        }

        // Set manually
        public CustomSerializableWrapper()
        {
            this.customItemSerializables = new List<CustomItemSerializable>();
            this.customFloorSerializables = new List<CustomFloorSerializable>();
            this.customTileableSerializables = new List<CustomTileableSerializable>();
        }
    }
}
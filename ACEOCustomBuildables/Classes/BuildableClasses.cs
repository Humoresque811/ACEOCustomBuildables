using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace ACEOCustomBuildables
{
    public class BuildableMod
    {
        public bool enabled;
        public string name;
        public string id;
        public string description;
        public string buildMenu;
        public string pathToUse; //This is used and created by JSON Manager, not to be inputted
    }

    public class TexturedBuildableMod : BuildableMod
    {
        public string texturePath;
        public string iconPath;
    }

    public class ShadowedBuildableMod : TexturedBuildableMod
    {
        public string shadowPath;
        public float shadowDistance;
        public float shadowTextureSizeMultiplier = 1.0f;
    }

    // Item mod
    public class ItemMod : ShadowedBuildableMod
    {
        public int x;
        public int y;
        public int buildCost;
        public int operationCost;
        public string itemPlacementArea;
        public bool useRandomRotation;
        public int constructionEnergy;
        public int contractors;
        public bool bogusOverride;
    }

    // Floor mod
    public class FloorMod : TexturedBuildableMod, IVariationMod
    {
        public string floorVariationId; // To be phased out, backwards support only
        public string variationID { get; set; }
    }

    // Tileable Mod
    public class TileableMod : TexturedBuildableMod, IVariationMod
    {
        public bool originalTexturePattern;
        public int buildCost;
        public int operationCost;
        public int tileSize;
        public string variationID { get; set; }
    }

    interface IVariationMod
    {
        string variationID { get; set; }
    }
}
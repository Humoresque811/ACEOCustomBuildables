using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

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
    public class FloorMod : TexturedBuildableMod
    {
        public string floorVariationId;
    }

    // Tileable Mod
    public class TileableMod : TexturedBuildableMod
    {
        public bool originalTexturePattern;
        public int buildCost;
        public int operationCost;
        public int tileSize;
    }
}
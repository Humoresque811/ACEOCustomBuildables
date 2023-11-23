using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ACEOCustomBuildables
{
    class FloorCreator : MonoBehaviour, IBuildableCreator
    {
        public static FloorCreator Instance { get; private set; }
        public List<GameObject> buildables { get; set; }

        public void SetUp()
        {
            Instance = this;
            buildables = new List<GameObject>();
        }

        public void ClearBuildables()
        {
            buildables = new List<GameObject>();
        }

        public void CreateBuildables()
        {
            buildables = new List<GameObject>();

            for (int i = 0; i < FileManager.Instance.buildableTypes[typeof(FloorMod)].Item2.buildableMods.Count; i++)
            {
                FloorMod floorMod = FileManager.Instance.buildableTypes[typeof(FloorMod)].Item2.buildableMods[i] as FloorMod;
                if (floorMod == null)
                {
                    ACEOCustomBuildables.Log("[Mod Error] A buildable mod of FloorCreators is not an floor mod!");
                    continue;
                }

                try
                {
                    GameObject template = TemplateManager.FloorTemplate;

                    GameObject newFloor = GameObject.Instantiate(template, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

                    if (!newFloor.TryGetComponent<PlaceableFloor>(out PlaceableFloor placeableFloor))
                    {
                        ACEOCustomBuildables.Log("[Mod Error] Failed to get PlaceableFloor component of floor!");
                        continue;
                    }
                    placeableFloor.structureType = Enums.StructureType.HardFloor;
                    placeableFloor.Quality = (byte)(FileManager.Instance.floorIndexAddative + i);
                    placeableFloor.snapSize = 1;
                    placeableFloor.objectGridSize = new Vector2(1, 1);
                    placeableFloor.objectType = Enums.ObjectType.Structure;
                    placeableFloor.objectSize = Enums.ThreeStepScale.Medium;
                    placeableFloor.objectQuality = Enums.QualityType.Medium;
                    placeableFloor.hasVariations = true;
                    placeableFloor.variationIndex = FileManager.Instance.floorIndexAddative + i;
                    placeableFloor.descriptionSizeModifier = 0.1f;

                    placeableFloor.objectName = floorMod.name;
                    placeableFloor.objectDescription = floorMod.description;

                    FileManager.Instance.GetIconSprite(floorMod, out Sprite iconSprite);
                    placeableFloor.descriptionSpriteOverrider = iconSprite;

                    CustomItemSerializableComponent comp = newFloor.AddComponent<CustomItemSerializableComponent>();
                    comp.Setup(i, typeof(FloorMod));

                    if (!newFloor.transform.GetChild(0).TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
                    {
                        ACEOCustomBuildables.Log("[Mod Error] Failed to get SpriteRenderer component of floor!");
                        continue;
                    }

                    FileManager.Instance.GetTextureSprite(floorMod, out Sprite sprite, 256);
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                    spriteRenderer.size = new Vector2(1, 1);
                    newFloor.transform.GetChild(0).localScale = new Vector3(1, 1, 1); //ItemCreator.Instance.calculateScale(newFloor.transform.GetChild(0).gameObject, 1f, 1f);

                    buildables.Add(newFloor);
                    newFloor.SetActive(false);

                    ACEOCustomBuildables.Log("[Mod Success] Created buildable floor \"" + floorMod.name + "\" successfully!");

                    newFloor = null;
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Creating floor \"" + floorMod.name + "\" failed. Error: " + ex.Message);
                }
            }
        }
    }
}

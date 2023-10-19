using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace ACEOCustomBuildables
{
    class ItemCreator : MonoBehaviour, IBuildableCreator
    {
        public static ItemCreator Instance { get; private set; }

        public List<GameObject> buildables { get; set; }
        private GameObject newBuildable;
        public bool itemsFailed = false;

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

            for (int i = 0; i < FileManager.Instance.buildableTypes[typeof(ItemMod)].Item2.buildableMods.Count; i++)
            {
                ItemMod itemMod = FileManager.Instance.buildableTypes[typeof(ItemMod)].Item2.buildableMods[i] as ItemMod;
                if (itemMod == null)
                {
                    ACEOCustomBuildables.Log("[Mod Error] A buildable mod of ItemSourceCreators is not an item mod!");
                    continue;
                }

                try
                {
                    // Get template item
                    GameObject template = TemplateManager.ItemTemplate;

                    // Start proccess
                    newBuildable = Instantiate(template, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

                    newBuildable = ConvertPlaceableItemIntoCustom(newBuildable, itemMod, i, 0);


                    // Do scaling
                    Transform spritesParent = newBuildable.transform.GetChild(0);
                    int x = itemMod.x;
                    int y = itemMod.y;
                    float multiplyer = itemMod.shadowTextureSizeMultiplier;
                    spritesParent.GetChild(0).localScale = calculateScale(spritesParent.GetChild(0).gameObject, x, y); // texture
                    spritesParent.GetChild(1).localScale = calculateScale(spritesParent.GetChild(1).gameObject, x * multiplyer, y * multiplyer); // shadow

                    if (newBuildable == null)
                    {
                        ACEOCustomBuildables.Log("[Mod Error] New Buildable \"" + itemMod.name + "\" is null!");
                        continue;
                    }

                    buildables.Add(newBuildable);
                    newBuildable.SetActive(false);

                    ACEOCustomBuildables.Log("[Mod Success] Created buildable item \"" + itemMod.name + "\" successfully");

                    newBuildable = null;
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Creating buildable \"" + itemMod.name + "\" failed. Error: " + ex.Message);
                    itemsFailed = true;
                }
            }

            // Post creation variable edits
            itemsFailed = false;
        }

        public void ConvertItemToCustom(GameObject item, int index, bool useRandomRotation, float spriteRotation, float itemRotation)
        {
            string internalLog = ""; // This is for logging, to show how far the code has got. Will only be logged upon a code failure
            ItemMod itemMod = FileManager.Instance.buildableTypes[typeof(ItemMod)].Item2.buildableMods[index] as ItemMod;
            if (itemMod == null)
            {
                ACEOCustomBuildables.Log("[Mod Error] A buildable mod of itemSourceCreators is not an item mod!");
                return;
            }

            try
            {
                if (item == null || index <= -1)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Item provided to convertItemToCustom is null or the index provided is less than one!");
                    return;
                }

                PlaceableItem pli = item.GetComponent<PlaceableItem>();
                SingletonNonDestroy<GridController>.Instance.RemoveReferenceFromMainGrid(pli.GetAllBorderPositions(), pli.ReferenceBytes);
                internalLog += "\nFinished pre-converstion edits";

                ConvertPlaceableItemIntoCustom(item, itemMod, index, itemRotation);
                internalLog += "\nFinsihed conversion";

                // Materials
                GameObject sprite = item.transform.GetChild(0).GetChild(0).gameObject;
                sprite.GetComponent<SpriteRenderer>().material = getSpriteNonLitMaterial(); // Sprite
                item.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().material = getSpriteNonLitMaterial(); // Overlay
                item.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material = getSpriteNonLitMaterial(); // Construction wireframe
                internalLog += "\nFinished material changes";

                // Rotation
                if (!useRandomRotation)
                {
                    sprite.transform.eulerAngles = new Vector3(0, 0, spriteRotation);
                }
                internalLog += "\nFinished rotation changes";

                // For scaling the textures
                Transform spritesParent = item.transform.GetChild(0);
                int x = itemMod.x;
                int y = itemMod.y;
                float multiplyer = itemMod.shadowTextureSizeMultiplier;
                spritesParent.GetChild(0).localScale = calculateScale(spritesParent.GetChild(0).gameObject, x, y); // texture
                spritesParent.GetChild(1).localScale = calculateScale(spritesParent.GetChild(1).gameObject, x * multiplyer, y * multiplyer); // shadow
                internalLog += "\nFinished texture scaling";


                // Adjusting corners, since it's allready placed
                item.transform.GetChild(2).GetChild(0).GetChild(0).transform.position += item.transform.position; // Neg Corner
                item.transform.GetChild(2).GetChild(0).GetChild(1).transform.position += item.transform.position; // Pos Corner
                item.transform.GetChild(2).GetChild(1).GetChild(0).transform.position += item.transform.position; // Neg Corner
                item.transform.GetChild(2).GetChild(1).GetChild(1).transform.position += item.transform.position; // pos Corner
                internalLog += "\nFinished corner pos edits";

                // Grid updates and changes
                pli.SetZOnFloor();  
                pli.ShouldShowOnFloor(pli.Floor);
                GridManager.AddObjectToMainGrid(pli.GetAllBorderPositions(), pli.boundary.penalty, pli.ReferenceBytes);
                GridManager.UpdateWalkableStatusOnPositions(pli.GetAllBorderPositions());
                internalLog += "\nFinished grid updates";


                // Changes for multi-floor stuff - sprite
                pli.CheckIfPlacedInside();
                pli.ShowHide(FloorManager.currentFloor);

                // Multi-Floor changes for overlay - Order matters, above MUST be done first.
                pli.overlay.UpdateOverlaySpriteMaterial(pli.isInside);
                if (pli.isInside)
                {
                    pli.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sortingLayerName = "BelowObjects";
                    Traverse.Create(pli.overlay).Field<SpriteRenderer>("mainOverlaySprite").Value.sortingLayerName = "SpriteOverlay";
                    Traverse.Create(pli.overlay).Field<SpriteRenderer>("constructionOverlaySprite").Value.sortingLayerName = "SpriteOverlay";
                }
                else
                {
                    // Nessesary idk, but just in case ;)
                    pli.overlay.SetOverlayLayerToStructureOutside();
                }
                internalLog += "\nFinished Multi-Floor edits";
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] An error occured while converting an item to custom. Info:" +
                    "\nError: " + ex.Message + 
                    "\nCustomItemName: " + itemMod.name + ", postion: " + item.transform.position.ToString() + 
                    "\nError Debug Log: " + internalLog);
            }
        }

        private GameObject ConvertPlaceableItemIntoCustom(GameObject item, ItemMod itemMod, int index, float itemRotation)
        {
            if (item == null || itemMod == null)
            {
                return null;
            }

            // For storing info/indentifying Mods
            CustomItemSerializableComponent itemsCustomSerializableInfo = item.gameObject.AddComponent<CustomItemSerializableComponent>();
            itemsCustomSerializableInfo.Setup(index, typeof(ItemMod));

            // Core Sprite Changes p1
            GameObject sprite = item.transform.GetChild(0).gameObject;
            GameObject overlay = item.transform.GetChild(1).gameObject;
            sprite.transform.rotation = new Quaternion(0, 0, 0, 0);
            sprite.GetComponent<SpriteRenderer>().enabled = false;

            // Core Sprite Changes p2
            GameObject texture = sprite.transform.GetChild(0).gameObject;
            GameObject shadow = sprite.transform.GetChild(1).gameObject;

            FileManager.Instance.GetTextureSprite(itemMod, out Sprite textureSprite, 256);
            texture.GetComponent<SpriteRenderer>().sprite = textureSprite;

            FileManager.Instance.GetShadowSprite(itemMod, out Sprite shadowSprite);
            shadow.GetComponent<SpriteRenderer>().sprite = shadowSprite;

            // Edit odd overlay thing (IDK what it does...?)
            overlay.transform.GetChild(0).localScale = new Vector3(itemMod.x, itemMod.y, 1);

            // Changes the wireframe overlay appropriatly, making sure that it is in tiled mode
            SpriteRenderer wireframeSpriteRenderer = overlay.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            wireframeSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
            wireframeSpriteRenderer.size = new Vector2(itemMod.x, itemMod.y);

            GameObject boundary = item.transform.GetChild(2).gameObject;
            Vector3 negCorner;
            Vector3 posCorner;
            if (itemRotation == 0 || itemRotation == 180)
            {
                negCorner = new Vector3((float)-Decimal.Divide(itemMod.x, 2), (float)-Decimal.Divide(itemMod.y, 2), 0); // Needs 90 degreee adjustment!!
                posCorner = new Vector3((float)Decimal.Divide(itemMod.x, 2), (float)Decimal.Divide(itemMod.y, 2), 0);
            }
            else
            {
                negCorner = new Vector3((float)-Decimal.Divide(itemMod.y, 2), (float)-Decimal.Divide(itemMod.x, 2), 0); // Needs 90 degreee adjustment!!
                posCorner = new Vector3((float)Decimal.Divide(itemMod.y, 2), (float)Decimal.Divide(itemMod.x, 2), 0);
            }

            boundary.transform.GetChild(0).GetChild(0).transform.position = negCorner;
            boundary.transform.GetChild(0).GetChild(1).transform.position = posCorner;

            boundary.transform.GetChild(1).GetChild(0).transform.position = negCorner;
            boundary.transform.GetChild(1).GetChild(1).transform.position = posCorner;

            // Script related chagnes
            PlaceableItem newBuildablePI = item.GetComponent<PlaceableItem>();
            newBuildablePI.hasVariations = false;
            newBuildablePI.objectName = itemMod.name;
            newBuildablePI.objectDescription = itemMod.description;
            newBuildablePI.objectCost = itemMod.buildCost;
            newBuildablePI.operationsCost = itemMod.operationCost;
            newBuildablePI.snapOffset = calculatOffest(itemMod);
            newBuildablePI.constructionEnergyRequired = itemMod.constructionEnergy;
            newBuildablePI.nbrOfContractorsPossible = itemMod.contractors;
            newBuildablePI.colorableSprites = new SpriteRenderer[0]; // Disables recoloring
            newBuildablePI.isRightClickable = false;

            if (itemRotation == 0 || itemRotation == 180)
            {
                newBuildablePI.objectGridSize = new Vector2(itemMod.x, itemMod.y);
            }
            else
            {
                newBuildablePI.objectGridSize = new Vector2(itemMod.y, itemMod.x);
            }

            // Where you can place it
            Enums.ItemPlacementArea itemPlacementArea;
            bool parsedSuccsefully = Enum.TryParse<Enums.ItemPlacementArea>(itemMod.itemPlacementArea, out itemPlacementArea);
            if (parsedSuccsefully)
            {
                newBuildablePI.itemPlacementArea = itemPlacementArea;
            }
            else
            {
                newBuildablePI.itemPlacementArea = Enums.ItemPlacementArea.Both;
            }
            
            // Shadow Stuff 
            if (item.transform.GetChild(0).GetChild(1).TryGetComponent<ShadowHandler>(out ShadowHandler shadowHandler))
            {
                shadowHandler.shadowDistance = itemMod.shadowDistance;
                shadowHandler.SetShadowSprite(shadow.GetComponent<SpriteRenderer>().sprite);
                //shadowHandler.UpdateShadow(); BROKEN AS OF ACEO 1.1 (Custom buildables 1.1.1 fixed)
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Error] There is no shadow handler...? Item: " + itemMod.name);
            }
            return item;
        }

        private Vector2 calculatOffest(ItemMod itemMod)
        {
            Vector2 offset = Vector2.zero; 
            if (itemMod.x % 2 == 0)
            {
                offset.x = 0.5f;
            }
            if (itemMod.y % 2 == 0)
            {
                offset.y = 0.5f;
            }
            return offset;
        }

        public Vector3 calculateScale(GameObject texture, float x_input, float y_input)
        {
            Bounds textureBounds = texture.GetComponent<SpriteRenderer>().bounds;
            Vector2 bounds = new Vector2(textureBounds.size.x, textureBounds.size.y);

            bounds.x = (float)Math.Round(bounds.x, 2);
            bounds.y = (float)Math.Round(bounds.y, 2);

            float x = x_input;
            float y = y_input;

            if (texture.transform.rotation.eulerAngles.z == 90 || texture.transform.rotation.eulerAngles.z == 270)
            {
                x = y_input;
                y = x_input;
            }

            float scaleValueX = 1f;
            float scaleValueY = 1f;

            if (x == y)
            {
                scaleValueX = x / bounds.x;
                scaleValueY = y / bounds.y;
            }
            else if (x > y)
            {
                // Cap the scale
                scaleValueY = y / bounds.y;
                scaleValueX = scaleValueY;
            }
            else if (y > x)
            {
                // cap the scale
                scaleValueX = x / bounds.x;
                scaleValueY = scaleValueX;
            }
            return new Vector3(scaleValueX, scaleValueY, 1f);
        }   

        private Material getSpriteNonLitMaterial()
        {
            return Singleton<BuildingController>.Instance.smallPlant.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material;
        }
	}
} 
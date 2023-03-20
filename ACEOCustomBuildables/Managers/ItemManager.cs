using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ACEOCustomBuildables
{
    class ItemManager : MonoBehaviour
    {
        public static GameObject newBuildable;
        public static List<GameObject> buildableModItems = new List<GameObject>();
        public static bool itemsCreated = false;
        public static bool itemsFailed = false;

        public static void clearBuildables()
        {
            for (int i = 0; i < buildableModItems.Count; i++)
            {
                buildableModItems.RemoveAt(i);
            }

            itemsCreated = false;
        }

        public static void createBuildables()
        {
            if (itemsCreated)
            {
                return;
            }

            for (int i = 0; i < JSONManager.itemMods.Count; i++)
            {
                try
                {
                    // Get template item
                    GameObject template = TemplateManager.ItemTemplate;

                    // Start proccess
                    newBuildable = Instantiate(template, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

                    newBuildable = convertPlaceableItemIntoCustom(newBuildable, i, 0);

                    // Do scaling
                    Transform spritesParent = newBuildable.transform.GetChild(0);
                    int x = JSONManager.itemMods[i].x;
                    int y = JSONManager.itemMods[i].y;
                    float multiplyer = JSONManager.itemMods[i].shadowTextureSizeMultiplier;
                    spritesParent.GetChild(0).localScale = calculateScale(spritesParent.GetChild(0).gameObject, x, y); // texture
                    spritesParent.GetChild(1).localScale = calculateScale(spritesParent.GetChild(1).gameObject, x * multiplyer, y * multiplyer); // shadow

                    if (newBuildable == null)
                    {
                        ACEOCustomBuildables.Log("[Mod Error] New Buildable \"" + JSONManager.itemMods[i].name + "\"is null!");
                    }

                    buildableModItems.Add(newBuildable);
                    newBuildable.SetActive(false);

                    ACEOCustomBuildables.Log("[Mod Success] Created buildable item \"" + JSONManager.itemMods[i].name + "\" successfully");

                    newBuildable = null;
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Creating buildable \"" + JSONManager.itemMods[i].name + "\" failed. Error: " + ex.Message);
                    itemsFailed = true;
                }
            }

            // Post creation variable edits
            itemsCreated = true;
            itemsFailed = false;
        }

        public static void convertItemToCustom(GameObject item, int index, bool useRandomRotation, float spriteRotation, float itemRotation)
        {
            string internalLog = ""; // This is for logging, to show how far the code has got. Will only be logged upon a code failure
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

                convertPlaceableItemIntoCustom(item, index, itemRotation);
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
                int x = JSONManager.itemMods[index].x;
                int y = JSONManager.itemMods[index].y;
                float multiplyer = JSONManager.itemMods[index].shadowTextureSizeMultiplier;
                spritesParent.GetChild(0).localScale = calculateScale(spritesParent.GetChild(0).gameObject, x, y); // texture
                spritesParent.GetChild(1).localScale = calculateScale(spritesParent.GetChild(1).gameObject, x * multiplyer, y * multiplyer); // shadow
                internalLog += "\nFinished texture scaling";

                // Adjusting corners, since it's allready placed
                item.transform.GetChild(2).GetChild(0).GetChild(0).transform.position += item.transform.position; // Neg Corner
                item.transform.GetChild(2).GetChild(0).GetChild(1).transform.position += item.transform.position; // Pos Corner
                item.transform.GetChild(2).GetChild(1).GetChild(0).transform.position += item.transform.position; // Neg Corner
                item.transform.GetChild(2).GetChild(1).GetChild(1).transform.position += item.transform.position; // pos Corner
                internalLog += "\nFinished corner pos edits";

                GridManager.AddObjectToMainGrid(pli.GetAllBorderPositions(), pli.boundary.penalty, pli.ReferenceBytes);
                GridManager.UpdateWalkableStatusOnPositions(pli.GetAllBorderPositions());
                internalLog += "\nFinished grid updates";
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] An error occured while converting an item to custom. Info:" +
                    "\nError: " + ex.Message + 
                    "\nCustomItemName: " + JSONManager.itemMods[index].name + ", postion: " + item.transform.position.ToString() + 
                    "\nError Debug Log: " + internalLog);
            }
        }

        private static GameObject convertPlaceableItemIntoCustom(GameObject item, int index, float itemRotation)
        {
            if (item == null || index <= -1)
            {
                return null;
            }

            // For storing info/indentifying Mods
            CustomItemSerializableComponent itemsCustomSerializableInfo = item.gameObject.AddComponent<CustomItemSerializableComponent>();
            itemsCustomSerializableInfo.index = index;

            // Core Sprite Changes p1
            GameObject sprite = item.transform.GetChild(0).gameObject;
            GameObject overlay = item.transform.GetChild(1).gameObject;
            sprite.transform.rotation = new Quaternion(0, 0, 0, 0);
            sprite.GetComponent<SpriteRenderer>().enabled = false;

            // Core Sprite Changes p2
            GameObject texture = sprite.transform.GetChild(0).gameObject;
            GameObject shadow = sprite.transform.GetChild(1).gameObject;
            texture.GetComponent<SpriteRenderer>().sprite = JSONManager.getSpriteFromPath(index, "Texture");
            shadow.GetComponent<SpriteRenderer>().sprite = JSONManager.getSpriteFromPath(index, "Shadow");

            // Edit odd overlay thing (IDK what it does...?)
            overlay.transform.GetChild(0).localScale = new Vector3(JSONManager.itemMods[index].x, JSONManager.itemMods[index].y, 1);

            // Changes the wireframe overlay appropriatly, making sure that it is in tiled mode
            SpriteRenderer wireframeSpriteRenderer = overlay.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            wireframeSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
            wireframeSpriteRenderer.size = new Vector2(JSONManager.itemMods[index].x, JSONManager.itemMods[index].y);

            GameObject boundary = item.transform.GetChild(2).gameObject;
            Vector3 negCorner;
            Vector3 posCorner;
            if (itemRotation == 0 || itemRotation == 180)
            {
                negCorner = new Vector3((float)-Decimal.Divide(JSONManager.itemMods[index].x, 2), (float)-Decimal.Divide(JSONManager.itemMods[index].y, 2), 0); // Needs 90 degreee adjustment!!
                posCorner = new Vector3((float)Decimal.Divide(JSONManager.itemMods[index].x, 2), (float)Decimal.Divide(JSONManager.itemMods[index].y, 2), 0);
            }
            else
            {
                negCorner = new Vector3((float)-Decimal.Divide(JSONManager.itemMods[index].y, 2), (float)-Decimal.Divide(JSONManager.itemMods[index].x, 2), 0); // Needs 90 degreee adjustment!!
                posCorner = new Vector3((float)Decimal.Divide(JSONManager.itemMods[index].y, 2), (float)Decimal.Divide(JSONManager.itemMods[index].x, 2), 0);
            }

            boundary.transform.GetChild(0).GetChild(0).transform.position = negCorner;
            boundary.transform.GetChild(0).GetChild(1).transform.position = posCorner;

            boundary.transform.GetChild(1).GetChild(0).transform.position = negCorner;
            boundary.transform.GetChild(1).GetChild(1).transform.position = posCorner;

            // Script related chagnes
            PlaceableItem newBuildablePI = item.GetComponent<PlaceableItem>();
            newBuildablePI.hasVariations = false;
            newBuildablePI.objectName = JSONManager.itemMods[index].name;
            newBuildablePI.objectDescription = JSONManager.itemMods[index].description;
            newBuildablePI.objectCost = JSONManager.itemMods[index].buildCost;
            newBuildablePI.operationsCost = JSONManager.itemMods[index].operationCost;
            newBuildablePI.snapOffset = calculatOffest(index);
            newBuildablePI.constructionEnergyRequired = JSONManager.itemMods[index].constructionEnergy;
            newBuildablePI.nbrOfContractorsPossible = JSONManager.itemMods[index].contractors;
            newBuildablePI.colorableSprites = new SpriteRenderer[0]; // Disables recoloring

            if (itemRotation == 0 || itemRotation == 180)
            {
                newBuildablePI.objectGridSize = new Vector2(JSONManager.itemMods[index].x, JSONManager.itemMods[index].y);
            }
            else
            {
                newBuildablePI.objectGridSize = new Vector2(JSONManager.itemMods[index].y, JSONManager.itemMods[index].x);
            }

            // Where you can place it
            Enums.ItemPlacementArea itemPlacementArea;
            bool parsedSuccsefully = Enum.TryParse<Enums.ItemPlacementArea>(JSONManager.itemMods[index].itemPlacementArea, out itemPlacementArea);
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
                shadowHandler.shadowDistance = JSONManager.itemMods[index].shadowDistance;
                shadowHandler.SetShadowSprite(shadow.GetComponent<SpriteRenderer>().sprite);
                shadowHandler.UpdateShadow();
            }
            else
            {
                ACEOCustomBuildables.Log("[Mod Error] There is no shadow handler...? Item: " + JSONManager.itemMods[index].name);
            }
            return item;
        }

        private static Vector2 calculatOffest(int index)
        {
            Vector2 offset = Vector2.zero; 
            if (JSONManager.itemMods[index].x % 2 == 0)
            {
                offset.x = 0.5f;
            }
            if (JSONManager.itemMods[index].y % 2 == 0)
            {
                offset.y = 0.5f;
            }
            return offset;
        }

        public static Vector3 calculateScale(GameObject texture, float x_input, float y_input)
        {
            Bounds textureBounds = texture.GetComponent<SpriteRenderer>().bounds;
            Vector2 bounds = new Vector2(textureBounds.size.x, textureBounds.size.y);

            bounds.x = (float)Math.Round(bounds.x, 2);
            bounds.y = (float)Math.Round(bounds.y, 2);

            float x = x_input;
            float y = y_input;

            if (texture.transform.parent.parent.rotation.eulerAngles.z == 90 || texture.transform.parent.parent.rotation.eulerAngles.z == 270)
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

        private static Material getSpriteNonLitMaterial()
        {
            return Singleton<BuildingController>.Instance.smallPlant.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material;
        }
	}
} 
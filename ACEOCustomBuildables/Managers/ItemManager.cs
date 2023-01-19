using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

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

            for (int i = 0; i < JSONManager.buildableMods.Count; i++)
            {
                try
                {
                    // Get template item
                    GameObject template = TemplateManager.ItemTemplate;

                    // Start proccess
                    newBuildable = Instantiate(template, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

                    newBuildable = convertPlaceableItemIntoCustom(newBuildable, i, 0);

                    if (newBuildable == null)
                    {
                        ACEOCustomBuildables.Log("[Mod Error] New Buildable \"" + JSONManager.buildableMods[i].name + "\"is null!");
                    }

                    buildableModItems.Add(newBuildable);
                    newBuildable.SetActive(false);

                    ACEOCustomBuildables.Log("[Mod Success] Created buildable item \"" + JSONManager.buildableMods[i].name + "\" successfully");

                    newBuildable = null;
                }
                catch (Exception ex)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Creating buildable \"" + JSONManager.buildableMods[i].name + "\" failed. Error: " + ex.Message);
                    itemsFailed = true;
                }
            }

            // Post creation variable edits
            itemsCreated = true;
            itemsFailed = false;
        }

        public static void convertItemToCustom(GameObject item, int index, bool useRandomRotation, float sprietRotation, float itemRotation)
        {
            try
            {
                if (item == null || index <= -1)
                {
                    ACEOCustomBuildables.Log("[Mod Error] Item provided to convertItemToCustom is null or the index provided is less than one!");
                    return;
                }

                PlaceableItem pli = item.GetComponent<PlaceableItem>();
                SingletonNonDestroy<GridController>.Instance.RemoveReferenceFromMainGrid(pli.GetAllBorderPositions(), pli.ReferenceBytes);

                convertPlaceableItemIntoCustom(item, index, itemRotation);

                GameObject sprite = item.transform.GetChild(0).GetChild(0).gameObject;
                sprite.GetComponent<SpriteRenderer>().material = getSpriteNonLitMaterial(); // Sprite
                item.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().material = getSpriteNonLitMaterial(); // Overlay
                item.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material = getSpriteNonLitMaterial(); // Construction wireframe
                if (!useRandomRotation)
                {
                    sprite.transform.eulerAngles = new Vector3(0, 0, sprietRotation);
                }

                item.transform.GetChild(2).GetChild(0).GetChild(0).transform.position += item.transform.position; // Neg Corner
                item.transform.GetChild(2).GetChild(0).GetChild(1).transform.position += item.transform.position; // Pos Corner
                item.transform.GetChild(2).GetChild(1).GetChild(0).transform.position += item.transform.position; // Neg Corner
                item.transform.GetChild(2).GetChild(1).GetChild(1).transform.position += item.transform.position; // pos Corner

                GridManager.AddObjectToMainGrid(pli.GetAllBorderPositions(), pli.boundary.penalty, pli.ReferenceBytes);
                GridManager.UpdateWalkableStatusOnPositions(pli.GetAllBorderPositions());
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Error converting an item to custom. Error: " + ex.Message);
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

            // Texture scaling
            int x = JSONManager.buildableMods[index].x;
            int y = JSONManager.buildableMods[index].y;
            if (x == y)
            {
                texture.transform.localScale = new Vector3(x, y, 1);
                shadow.transform.localScale = new Vector3(x, y, 1);
            }
            else if (x > y)
            {
                texture.transform.localScale = new Vector3(y, y, 1);
                shadow.transform.localScale = new Vector3(y, y, 1);
            }
            else if (x < y)
            {
                texture.transform.localScale = new Vector3(x, x, 1);
                shadow.transform.localScale = new Vector3(x, x, 1);
            }

            // Edit odd overlay thing (IDK what it does...?)
            overlay.transform.GetChild(0).localScale = new Vector3(JSONManager.buildableMods[index].x, JSONManager.buildableMods[index].y, 1);

            // Changes the wireframe overlay appropriatly, making sure that it is in tiled mode
            SpriteRenderer wireframeSpriteRenderer = overlay.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            wireframeSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
            wireframeSpriteRenderer.size = new Vector2(JSONManager.buildableMods[index].x, JSONManager.buildableMods[index].y);

            GameObject boundary = item.transform.GetChild(2).gameObject;
            Vector3 negCorner;
            Vector3 posCorner;
            if (itemRotation == 0 || itemRotation == 180)
            {
                negCorner = new Vector3((float)-Decimal.Divide(JSONManager.buildableMods[index].x, 2), (float)-Decimal.Divide(JSONManager.buildableMods[index].y, 2), 0); // Needs 90 degreee adjustment!!
                posCorner = new Vector3((float)Decimal.Divide(JSONManager.buildableMods[index].x, 2), (float)Decimal.Divide(JSONManager.buildableMods[index].y, 2), 0);
            }
            else
            {
                negCorner = new Vector3((float)-Decimal.Divide(JSONManager.buildableMods[index].y, 2), (float)-Decimal.Divide(JSONManager.buildableMods[index].x, 2), 0); // Needs 90 degreee adjustment!!
                posCorner = new Vector3((float)Decimal.Divide(JSONManager.buildableMods[index].y, 2), (float)Decimal.Divide(JSONManager.buildableMods[index].x, 2), 0);

            }

            boundary.transform.GetChild(0).GetChild(0).transform.position = negCorner;
            boundary.transform.GetChild(0).GetChild(1).transform.position = posCorner;

            boundary.transform.GetChild(1).GetChild(0).transform.position = negCorner;
            boundary.transform.GetChild(1).GetChild(1).transform.position = posCorner;

            // Script related chagnes
            PlaceableItem newBuildablePI = item.GetComponent<PlaceableItem>();
            newBuildablePI.hasVariations = false;
            newBuildablePI.objectName = JSONManager.buildableMods[index].name;
            newBuildablePI.objectDescription = JSONManager.buildableMods[index].description;
            newBuildablePI.objectCost = JSONManager.buildableMods[index].buildCost;
            newBuildablePI.operationsCost = JSONManager.buildableMods[index].operationCost;
            newBuildablePI.snapOffset = calculatOffest(index);
            newBuildablePI.constructionEnergyRequired = JSONManager.buildableMods[index].constructionEnergy;
            newBuildablePI.nbrOfContractorsPossible = JSONManager.buildableMods[index].contractors;

            if (itemRotation == 0 || itemRotation == 180)
            {
                newBuildablePI.objectGridSize = new Vector2(JSONManager.buildableMods[index].x, JSONManager.buildableMods[index].y);
            }
            else
            {
                newBuildablePI.objectGridSize = new Vector2(JSONManager.buildableMods[index].y, JSONManager.buildableMods[index].x);
            }
            //newBuildablePI.const

            // Where you can place it
            Enums.ItemPlacementArea itemPlacementArea;
            bool parsedSuccsefully = Enum.TryParse<Enums.ItemPlacementArea>(JSONManager.buildableMods[index].itemPlacementArea, out itemPlacementArea);
            if (parsedSuccsefully)
            {
                newBuildablePI.itemPlacementArea = itemPlacementArea;
            }
            else
            {
                newBuildablePI.itemPlacementArea = Enums.ItemPlacementArea.Both;
            }

            return item;
        }

        private static Vector2 calculatOffest(int index)
        {
            Vector2 offset = Vector2.zero; 
            if (JSONManager.buildableMods[index].x % 2 == 0)
            {
                offset.x = 0.5f;
            }
            if (JSONManager.buildableMods[index].y % 2 == 0)
            {
                offset.y = 0.5f;
            }
            return offset;
        }

        private static Vector3[] calculateAllPostions(int x_width, int y_height, Vector3 center)
        {
            Vector3[] returnArray = new Vector3[(x_width * y_height)];
            float verticalHalf = (float) Decimal.Divide(y_height, 2);
            float horizonatlHalf = (float) Decimal.Divide(x_width, 2);
            float verticalAdd = -0.5f; // y_height % 2 == 0 ? 0.5f : 0;
            float horizontalAdd = -0.5f; // x_width % 2 == 0 ? 0.5f : 0;

            for (int x = 0; x < x_width; x++)
            {
                for (int y = 0; y < y_height; y++)
                {
                    returnArray[(x * (x_width - 1)) + y].x = x - horizonatlHalf - horizontalAdd + center.x;
                    returnArray[(x * (x_width - 1)) + y].y = y - verticalHalf - verticalAdd + center.y;
                }
            }

            return returnArray;
        }

        private static Material getSpriteNonLitMaterial()
        {
            return Singleton<BuildingController>.Instance.smallPlant.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material;
        }
	}
} 
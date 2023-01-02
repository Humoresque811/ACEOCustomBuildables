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

                    // For storing info/indentifying Mods
                    CustomItemSerializableInfo itemsCustomSerializableInfo = newBuildable.gameObject.AddComponent<CustomItemSerializableInfo>();
                    itemsCustomSerializableInfo.setModInfoFromClass(JSONManager.buildableMods[i]);

                    // Core Sprite Changes p1
                    GameObject sprite = newBuildable.transform.GetChild(0).gameObject;
                    GameObject overlay = newBuildable.transform.GetChild(1).gameObject;
                    sprite.transform.rotation = new Quaternion(0, 0, 0, 0);
                    sprite.GetComponent<SpriteRenderer>().enabled = false;

                    // Core Sprite Changes p2
                    GameObject texture = sprite.transform.GetChild(0).gameObject;
                    GameObject shadow = sprite.transform.GetChild(1).gameObject;
                    texture.GetComponent<SpriteRenderer>().sprite = JSONManager.getSpriteFromPath(i, "Texture");
                    shadow.GetComponent<SpriteRenderer>().sprite = JSONManager.getSpriteFromPath(i, "Shadow");

                    // Texture scaling
                    int x = JSONManager.buildableMods[i].x;
                    int y = JSONManager.buildableMods[i].y;
                    if (x == y)
                    {
                        texture.transform.localScale = new Vector3(x, x, 1);
                        shadow.transform.localScale = new Vector3(x, x, 1);
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
                    overlay.transform.GetChild(0).localScale = new Vector3(JSONManager.buildableMods[i].x, JSONManager.buildableMods[i].y, 1);

                    // Changes the wireframe overlay appropriatly, making sure that it is in tiled mode
                    SpriteRenderer wireframeSpriteRenderer = overlay.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                    wireframeSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
                    wireframeSpriteRenderer.size = new Vector2(JSONManager.buildableMods[i].x, JSONManager.buildableMods[i].y);

                    GameObject boundary = newBuildable.transform.GetChild(2).gameObject;
                    Vector3 negCorner = new Vector3((float)-Decimal.Divide(JSONManager.buildableMods[i].x, 2), (float)-Decimal.Divide(JSONManager.buildableMods[i].y, 2), 0);
                    Vector3 posCorner = new Vector3((float)Decimal.Divide(JSONManager.buildableMods[i].x, 2), (float)Decimal.Divide(JSONManager.buildableMods[i].y, 2), 0);

                    boundary.transform.GetChild(0).GetChild(0).transform.position = negCorner;
                    boundary.transform.GetChild(0).GetChild(1).transform.position = posCorner;

                    boundary.transform.GetChild(1).GetChild(0).transform.position = negCorner;
                    boundary.transform.GetChild(1).GetChild(1).transform.position = posCorner;

                    // Script related chagnes
                    PlaceableItem newBuildablePI = newBuildable.GetComponent<PlaceableItem>();
                    newBuildablePI.hasVariations = false;
                    newBuildablePI.objectName = JSONManager.buildableMods[i].name;
                    newBuildablePI.objectDescription = JSONManager.buildableMods[i].description;
                    newBuildablePI.objectCost = JSONManager.buildableMods[i].buildCost;
                    newBuildablePI.operationsCost = JSONManager.buildableMods[i].operationCost;
                    newBuildablePI.snapOffset = calculatOffest(i);
                    newBuildablePI.objectGridSize = new Vector2(JSONManager.buildableMods[i].x, JSONManager.buildableMods[i].y);

                    // Where you can place it
                    Enums.ItemPlacementArea itemPlacementArea;
                    bool parsedSuccsefully = Enum.TryParse<Enums.ItemPlacementArea>(JSONManager.buildableMods[i].itemPlacementArea, out itemPlacementArea);
                    if (parsedSuccsefully)
                    {
                        newBuildablePI.itemPlacementArea = itemPlacementArea;
                    }
                    else
                    {
                        newBuildablePI.itemPlacementArea = Enums.ItemPlacementArea.Both;
                    }

                    buildableModItems.Add(newBuildable);
                    newBuildable.SetActive(false);

                    ACEOCustomBuildables.Log("[Mod Success] Created buildable item \"" + JSONManager.buildableMods[i].name + "\" successfully");

                    sprite = null;
                    overlay = null;
                    boundary = null;
                    newBuildablePI = null;
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
        
        static Vector2 calculatOffest(int index)
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
	}
} 
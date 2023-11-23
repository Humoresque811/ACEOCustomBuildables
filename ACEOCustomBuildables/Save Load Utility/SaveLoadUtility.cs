using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Nodes;
using System.Net;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ACEOCustomBuildables
{
    class SaveLoadUtility : MonoBehaviour
    {
        // Function vars
        static float gameTimeWhenSaving = 1f;

        // Buildable Arrays
        //public static List<PlaceableStructure> structureArray;
        public static List<TaxiwayNode> taxiwayNodesArray;
        public static DynamicSimpleArray<PlaceableItem> itemArray;
        public static MergedTile[] zonesArray;
        public static DynamicArray<PlaceableRoom> roomArray;
        public static AircraftController[] aircraftArray;
        public static VehicleController[] vehicleArray;
        public static TrailerController[] trailerArray;
        public static PersonController[] personArray;
        public static DynamicSimpleArray<AssetController> assetArray;

        // JSON vars
        [SerializeField] public static List<CustomItemSerializable> itemJSONList = new List<CustomItemSerializable>();
        [SerializeField] public static List<CustomFloorSerializable> floorJSONList = new List<CustomFloorSerializable>();
        [SerializeField] public static List<CustomTileableSerializable> tileableJSONList = new List<CustomTileableSerializable>();

        // Function Utilities
        public static void quicklog(string message, bool logAsUnityError = false)
        {
            if (logAsUnityError)
            {
                Debug.LogError("[SaveLoadUtility] " + message);
            }
            ACEOCustomBuildables.Log("[SaveLoadUtility] " + message);
        }


        // Interaction Utilities
        // Future Update ;)


        // Game Utilities
        public static void makeGameReadyForSaving()
        {
            try
            {
                // Removes player interaction and object movement
                Singleton<MainInteractionPanelUI>.Instance.EnableDispableSavingTextPanel(true);
                gameTimeWhenSaving = Singleton<TimeController>.Instance.currentSpeed;
                PlayerInputController.SetPlayerControlAllowed(false);
                if (gameTimeWhenSaving != 0f)
                {
                    Singleton<TimeController>.Instance.TogglePauseTime();
                }
            }
            catch (Exception ex)
            {
                quicklog("Error making game ready for saving! Error: " + ex.Message, true);
            }
        }
        public static void revetGameAfterSaving()
        {
            try
            {
                // Allows player interaction and object movement
                if (gameTimeWhenSaving != 0f)
                {
                    Singleton<TimeController>.Instance.TogglePauseTime();
                }
                PlayerInputController.SetPlayerControlAllowed(true);
                if (gameTimeWhenSaving == 100f)
                {
                    Singleton<TimeController>.Instance.InvokeSkipToNextDay();
                }
                Singleton<MainInteractionPanelUI>.Instance.EnableDispableSavingTextPanel(false);
            }
            catch (Exception ex)
            {
                quicklog("Error reverting game after saving! Error: " + ex.Message, true);
            }
        }
        public static void getAllBuildablesArrays()
        {
            try
            {
                // Structure array still needs to be added, its more complicated
                taxiwayNodesArray = Singleton<TaxiwayController>.Instance.GetSerializableTaxiwayNodeList();
                zonesArray = TileMerger.mergedTiles.ToArray();
                roomArray = Singleton<BuildingController>.Instance.allRoomsArray;
                itemArray = Singleton<BuildingController>.Instance.allItemsArray;
                aircraftArray = Singleton<AirTrafficController>.Instance.GetAircraftList();
                vehicleArray = Singleton<TrafficController>.Instance.GetVehicleArray();
                trailerArray = Singleton<TrafficController>.Instance.GetTrailersArray();
                personArray = Singleton<AirportController>.Instance.GetAllPersons();
                assetArray = Singleton<AirportController>.Instance.allAssetsArray;
            }
            catch (Exception ex)
            {
                quicklog("Error getting buildable arrays! Error: " + ex.Message, true);
            }
        }

        public static CustomItemSerializable SetItemSerializableInfo(int index, PlaceableItem item)
        {
            try
            {
                // Transfer vars and nessesary info
                CustomItemSerializable returnItem = new CustomItemSerializable();
                returnItem.modId = ItemModSourceCreator.Instance.buildableMods[index].id;
                returnItem.spriteRotation = Mathf.Round(item.transform.GetChild(0).GetChild(0).transform.eulerAngles.z);
                returnItem.itemRotation = Mathf.Round(item.transform.eulerAngles.z);

                returnItem.postion[0] = item.transform.position.x;
                returnItem.postion[1] = item.transform.position.y;
                returnItem.postion[2] = item.transform.position.z;

                returnItem.floor = item.Floor;
                returnItem.referenceID = item.ReferenceID;
                return returnItem;
            }
            catch (Exception ex)
            {
                quicklog("Error occured in setting a serializable item class. Error: " + ex.Message, true);
                return null;
            }
        }

        public static CustomFloorSerializable SetFloorSeializableInfo(int index, MergedTile mergedTile)
        {
            try
            {
                CustomFloorSerializable returnFloor = new CustomFloorSerializable();
                returnFloor.modId = FloorModSourceCreator.Instance.buildableMods[index].id;

                returnFloor.position[0] = mergedTile.transform.position.x;
                returnFloor.position[1] = mergedTile.transform.position.y;
                returnFloor.position[2] = mergedTile.transform.position.z;

                returnFloor.size[0] = mergedTile.spriteRenderer.size[0];
                returnFloor.size[1] = mergedTile.spriteRenderer.size[1];

                returnFloor.tileType = mergedTile.TileType.ToString();
                returnFloor.floor = mergedTile.Floor;

                return returnFloor;
            }
            catch (Exception ex)
            {
                quicklog("Error occured in setting a serializable floor class. Error: " + ex.Message, true);
                return null;
            }
        }

        public static CustomTileableSerializable SetTileableSerializableInfo(int index, PlaceableItem item)
        {
            try
            {
                // Transfer vars and nessesary info
                CustomTileableSerializable returnItem = new CustomTileableSerializable();
                returnItem.modId = TileableSourceCreator.Instance.buildableMods[index].id;

                returnItem.position[0] = item.transform.position.x;
                returnItem.position[1] = item.transform.position.y;
                returnItem.position[2] = item.transform.position.z;

                returnItem.floor = item.Floor;
                returnItem.referenceID = item.ReferenceID;
                return returnItem;
            }
            catch (Exception ex)
            {
                quicklog("Error occured in setting a serializable tileable class. Error: " + ex.Message, true);
                return null;
            }
        }

        // JSON making!
        /// <summary>
        /// Creates the JSON file for saveLoadUtility
        /// </summary>
        /// <param name="path">This *MUST* be the save courtine's own input!</param>
        /// <param name="fileName">If you want to use a custom file name, specify it</param>
        public static void CreateJSON(string path, string fileName = "CustomSaveData.json")
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // Make sure the array isn't empty!
            if (!((itemJSONList != null && itemJSONList.Count != 0) || (floorJSONList != null && floorJSONList.Count != 0) || (tileableJSONList != null && tileableJSONList.Count != 0)))
            {
                return;
            }

            // Get basepath, add it if not allready there
            string basepath = Singleton<SaveLoadGameDataController>.Instance.GetUserSavedDataSearchPath();
            if (!string.Equals(path.SafeSubstring(0, 2), "C:"))
            {
                path = Path.Combine(basepath.Remove(basepath.Length - 1), path);
            }

            // Make sure the directory does exist
            if (!Directory.Exists(path))
            {
                quicklog("The directory for saving does not exist! The path was \"" + path + "\".", true);
                return;
            }

            // Add the filename itself
            path = Path.Combine(path, fileName);
            quicklog("Full path is \"" + path + "\"", false);

            // Make sure the file doesn't allready exist
            if (File.Exists(path))
            {
                quicklog("The file allready exists!", true);
                return;
            }


            // JSON creation vars and systems
            string JSON;
            CustomSerializableWrapper JSONWrapper = new CustomSerializableWrapper(itemJSONList, floorJSONList, tileableJSONList);
            try
            {
                JSON = JsonConvert.SerializeObject(JSONWrapper, Formatting.Indented); // We pretty print :)
            }
            catch (Exception ex)
            {
                quicklog("Error converting classes to JSON. Error: " + ex.Message, true);
                return;
            }


            // Saving
            string exception;
            try
            {
                Utils.TryWriteFile(JSON, path, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    quicklog("An exception occured! It was: " + exception, true);
                    return;
                }
            }
            catch (Exception ex)
            {
                quicklog("Outer error writing file to JSON. Error: " + ex.Message, true);
                return;
            }

            quicklog("JSON creation succesfull, and JSON saving finished! Yay", true);
        }
    }
}
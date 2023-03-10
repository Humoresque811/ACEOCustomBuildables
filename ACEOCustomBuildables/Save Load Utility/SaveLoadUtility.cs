using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Nodes;
using System.Net;
using Newtonsoft.Json;

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
        [SerializeField] public static List<CustomItemSerializable> JSONInfoArray = new List<CustomItemSerializable>(); // <-------------- Will be reconfigured in future update

        // Function Utilities
        void Start()
        {
            quicklog("Save Load Utility is online with instance set!");
        }
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

        public static CustomItemSerializable setSerializableInfo(int index, PlaceableItem item)
        {
            try
            {
                // Transfer vars and nessesary info
                CustomItemSerializable returnItem = new CustomItemSerializable();
                returnItem.modId = JSONManager.buildableMods[index].id;
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
                quicklog("Error occured it setting a serializable info class. Error: " + ex.Message, true);
                return null;
            }
        }

        /*public static float adjustFloatToRightDegreeIncrements(float input)
        {
            float output;

            if (input == 0 || input == 90 || input == 180 || input == 270)
            {
                return input;
            }

            Mathf.LerpAngle()
            return output;
        }*/


        // JSON making!
        /// <summary>
        /// Creates the JSON file for saveLoadUtility
        /// </summary>
        /// <param name="path">This *MUST* be the save courtine's own input!</param>
        /// <param name="fileName">If you want to use a custom file name, specify it</param>
        public static void createJSON(string path, string fileName = "CustomSaveData.json")
        {
            // Make sure the array isn't empty!
            if (JSONInfoArray == null || JSONInfoArray.Count == 0 || string.IsNullOrEmpty(path))
            {
                return;
            }

            // Get basepath, add it if not allready there
            string basepath = Singleton<SaveLoadGameDataController>.Instance.GetUserSavedDataSearchPath();
            if (!string.Equals(path.SafeSubstring(0, 2), "C:"))
            {
                path = basepath.Remove(basepath.Length - 1) + "\\" + path;
            }

            // Make sure the directory does exist
            if (!Directory.Exists(path))
            {
                quicklog("The directory for saving does not exist! The path was \"" + path + "\".", true);
                return;
            }

            // Add the filename itself
            path = path + "\\" + fileName;
            quicklog("Full path is \"" + path + "\"", false);

            // Make sure the file doesn't allready exist
            if (File.Exists(path))
            {
                quicklog("The file allready exists!", true);
                return;
            }


            // JSON creation vars and systems
            string JSON;
            CustomItemSerializableWrapper JSONWrapper = new CustomItemSerializableWrapper(JSONInfoArray);
            try
            {
                JSON = JsonConvert.SerializeObject(JSONWrapper, Formatting.Indented); // We pretty print :)
            }
            catch (Exception ex)
            {
                quicklog("Error Converting casses to JSON. Error: " + ex.Message, true);
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
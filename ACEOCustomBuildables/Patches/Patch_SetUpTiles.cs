using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(TiledObjectsManager), "AddTileable")]
    class Patch_SetUpTiles
    {
        public static void Prefix(TiledObjectsManager __instance, ITileable tileable)
        {
            if (tileable.Quality < FileManager.Instance.floorIndexAddative)
            {
                return;
            }

            if (tileable.TileType != Enums.TileType.Floor)
            {
                return;
            }

            Patch_LoadPostfix.LoadedWrapper = Patch_LoadPostfix.GetCustomSaveData(Patch_LoadPostfix.savePath);

            if (Patch_LoadPostfix.LoadedWrapper == null)
            {
                ACEOCustomBuildables.Log("[Mod Error] Loaded wrapper was null, so the setTile system can't set tiles! (If you see this error, then " +
                    "ACEO Custom Buildables has been added to a game, used, and then removed, then the game was saved, and then ACEO CB was re-added. This can cuase significant game damage!" +
                    " If you need help, contact Humoresque.)");
                return;
            }

            foreach (CustomFloorSerializable customFloor in Patch_LoadPostfix.LoadedWrapper.customFloorSerializables)
            {
                Vector3 customPosition = new Vector3(customFloor.position[0], customFloor.position[1], customFloor.position[2]);
                Vector2 customSize = new Vector2(customFloor.size[0], customFloor.size[1]);

                if (tileable.Position.x < customPosition.x - (float)Decimal.Divide((decimal)customSize.x, 2) || tileable.Position.x > customPosition.x + (float)Decimal.Divide((decimal)customSize.x, 2))
                {
                    continue;
                }

                if (tileable.Position.y < customPosition.y - (float)Decimal.Divide((decimal)customSize.y, 2) || tileable.Position.y > customPosition.y + (float)Decimal.Divide((decimal)customSize.y, 2))
                {
                    continue;
                }

                if (!int.Equals(customFloor.floor, tileable.Floor))
                {
                    continue;
                }


                // They are the same
                for (int i = 0; i < FloorModSourceCreator.Instance.buildableMods.Count; i++)
                {
                    FloorMod floorMod = FloorModSourceCreator.Instance.buildableMods[i] as FloorMod;
                    if (floorMod == null)
                    {
                        continue;
                    }

                    if (!string.Equals(floorMod.id, customFloor.modId))
                    {
                        if (i != FloorModSourceCreator.Instance.buildableMods.Count - 1)
                        {
                            continue;
                        }
                        // Now the problem is there isn't a matching ID!
                        SaveLoadUtility.quicklog("[Buildable Problem] A custom floor is in the processs of being loaded, but a custom floor with the id in the save file doesn't exist. " +
                            "This probably means that a mod was uninstalled or its ID was changed. To fix this, either re-install the mod or revert any changes made to a mods id. " +
                            "The id being looked for is " + customFloor.modId + ". For help, contact Humoresque.", false);
                        tileable.Quality = 0;
                        continue;
                    }

                    tileable.Quality = (byte) (i + FileManager.Instance.floorIndexAddative);
                    break;
                }

            }
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace ACEOCustomBuildables
{
    [HarmonyPatch(typeof(PlaceableFloor), "GetVariations")]
    class Patch_Variations
    {
        public static void Postfix(PlaceableFloor __instance, ref Variation[] __result)
        {
            try
            {
                if (!__instance.gameObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent comp))
                {
                    return;
                }

                ACEOCustomBuildables.Log(__instance.variationIndex.ToString());
                if (__instance.variationIndex < FileManager.Instance.floorIndexAddative)
                {
                    return;
                }

                int[] variationInts = UIManager.GetAllVariations(comp.floorIndex);
                List<Variation> variations = new List<Variation>();

                for (int i = 0; i < variationInts.Length; i++)
                {
                    FileManager.Instance.GetIconSprite(FloorModSourceCreator.Instance.buildableMods[variationInts[i]], out Sprite sprite, 256);
                    string text = FloorModSourceCreator.Instance.buildableMods[variationInts[i]].name;
                    variations.Add(new Variation(Enums.ThreeStepScale.Medium, Enums.QualityType.Medium, variationInts[i] + FileManager.Instance.floorIndexAddative, sprite, text));
                }

                __result = variations.ToArray();
            }
            catch (Exception ex)
            {
                ACEOCustomBuildables.Log("[Mod Error] Variation patch failed! Error: " + ex.Message);
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDescriptionPanelUI), "SetObjectImage")]
    class Patch_VariationSpriteScaler
    {
        public static void Prefix(ObjectDescriptionPanelUI __instance, VariationPackage variationPackage, out int __state)
        {
            __state = variationPackage.index;
        }

        public static void Postfix(ObjectDescriptionPanelUI __instance, int __state)
        {
            if (__state < FileManager.Instance.floorIndexAddative)
            {
                return;
            }

            __instance.objectImageTransform.localScale = new Vector3(64f, 64f, 64f);
        }
    }

    [HarmonyPatch(typeof(ObjectPlacementController), "SetObject")]
    class Path_VariationPlacementSpriteScaler
    {
        public static void Prefix(ObjectPlacementController __instance, GameObject obj)
        {
            if (!obj.TryGetComponent<PlaceableFloor>(out PlaceableFloor plo))
            {
                return;
            }

            if (!obj.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent comp))
            {
                if (plo.variationIndex < FileManager.Instance.floorIndexAddative)
                {
                    return;
                }

                obj = FloorCreator.Instance.buildables[plo.variationIndex - FileManager.Instance.floorIndexAddative];
                //comp = obj.AddComponent<CustomItemSerializableComponent>();
                //comp.Setup(plo.variationIndex - FileManager.Instance.floorIndexAddative, typeof(FloorMod));
            }

            Transform sprite = obj.transform.Find("Sprite");
            sprite.localScale = Vector3.one;
            sprite.localScale = ItemCreator.Instance.calculateScale(sprite.gameObject, 1, 1);
        }
    }

    [HarmonyPatch(typeof(ObjectDescriptionPanelUI), "SetText")]
    class Patch_SetCorrectDescription
    {
        public static void Prefix(ObjectDescriptionPanelUI __instance, PlaceableObject currentPlaceableObject, VariationPackage variationPackage, bool isVariation, out int __state)
        {
            PlaceableFloor placeableFloor = currentPlaceableObject as PlaceableFloor;
            if (placeableFloor == null)
            {
                __state = -1;
                return;
            }

            if (variationPackage.index < FileManager.Instance.floorIndexAddative)
            {
                __state = -1;
                return;
            }

            __state = variationPackage.index;
        }

        public static void Postfix(ObjectDescriptionPanelUI __instance, int __state)
        {
            if (__state == -1)
            {
                return;
            }

            __instance.objectDescriptionText.text = FloorModSourceCreator.Instance.buildableMods[__state - FileManager.Instance.floorIndexAddative].description;
        }
    }
}

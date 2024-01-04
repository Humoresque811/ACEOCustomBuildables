using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace ACEOCustomBuildables
{
    public static class BuildableClassHelper
    {
        public static string GetModIdentification(BuildableMod mod)
        {
            return $"\"{mod.name}\", with id \"{mod.id}\",";
        }

        public static bool GetAllBuildableMods(out List<TexturedBuildableMod> buildableMods)
        {
            if (FileManager.Instance.buildableTypes == null || FileManager.Instance.buildableTypes.Count == 0)
            {
                buildableMods = null;
                return false;
            }

            List<TexturedBuildableMod> output = new List<TexturedBuildableMod>();
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                output.AddRange(FileManager.Instance.buildableTypes[type].Item2.buildableMods);
            }

            buildableMods = output;
            return true;
        }
        public static List<GameObject> GetAllBuildables()
        {
            List<GameObject> output = new List<GameObject>();
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                output.AddRange(FileManager.Instance.buildableTypes[type].Item3.buildables);
            }

            return output;
        }
        public static bool GetBuildableCreator(Type modType, out IBuildableCreator buildableCreator)
        {
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                if (!Type.Equals(modType, type))
                {
                    continue;
                }

                buildableCreator = FileManager.Instance.buildableTypes[modType].Item3;
                return true;
            }

            buildableCreator = null;
            return false;
        }

        public static bool GetBuildableSourceCreator(Type modType, out IBuildableSourceCreator buildableSourceCreator)
        {
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                if (!Type.Equals(modType, type))
                {
                    continue;
                }

                buildableSourceCreator = FileManager.Instance.buildableTypes[modType].Item2;
                return true;
            }

            buildableSourceCreator = null;
            return false;
        }

        public static bool GetBuildablePathExtension(Type modType, out string pathExtension)
        {
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                if (!Type.Equals(modType, type))
                {
                    continue;
                }

                pathExtension = FileManager.Instance.buildableTypes[type].Item1;
                return true;
            }

            pathExtension = "";
            return false;
        }

        public static bool IsValidVariationModType(Type modType)
        {
            if (UIManager.variations.Keys.ToArray().Contains(modType))
            {
                return true;
            }

            return false;
        }
    }
}
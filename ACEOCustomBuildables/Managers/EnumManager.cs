using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACEOCustomBuildables
{
    public static class EnumManager
    {
        private static readonly int itemEnumBaseAddative = 100;

        private static readonly int itemEnumCustomItem = 0;
        private static readonly int itemEnumCustomTileable = 1;

        public static int ItemEnumCustomItem
        {
            get
            {
                return itemEnumBaseAddative + itemEnumCustomItem;
            }
        }

        public static int ItemEnumCustomTileable
        {
            get
            {
                return itemEnumBaseAddative + itemEnumCustomTileable;
            }
        }

        public static bool ValidateEnums(Action<string> logger)
        {
            if (logger == null)
            {
                ACEOCustomBuildables.Log("[Mod Error] Failed to check enums due to a null logger");
                return false;
            }

            if (itemEnumBaseAddative < Enum.GetNames(typeof(Enums.ItemType)).Length)
            {
                logger($"[Mod Error] Conflicting enums with vanilla ones due to improper addative value " +
                    $"(Addative value at {itemEnumBaseAddative} with lenght of enum being {Enum.GetNames(typeof(Enums.ItemType)).Length})");
                return false;
            }

            if (itemEnumCustomItem == itemEnumCustomTileable)
            {
                logger("[Mod Error] Duplicate custom enum values...");
                return false;
            }

            return true;
        }
    }
}

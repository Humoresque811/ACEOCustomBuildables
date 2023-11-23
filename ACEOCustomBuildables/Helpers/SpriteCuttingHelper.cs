using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ACEOCustomBuildables
{
    public static class SpriteCuttingHelper
    {
        // All are in amount OUT OF 60. The sprite is 60px wide, then the values are correct.
        private static Dictionary<Enums.BuilderPieceType, Vector2> originalSpriteCuttingPoints = new Dictionary<Enums.BuilderPieceType, Vector2>()
        {
            { Enums.BuilderPieceType.Straight, new Vector2(2, 0) },
            { Enums.BuilderPieceType.Turn, new Vector2(0, 0) },
            { Enums.BuilderPieceType.TTurn, new Vector2(1, 1) },
            { Enums.BuilderPieceType.XTurn, new Vector2(1, 0) },
            { Enums.BuilderPieceType.End, new Vector2(2, 1) },
            { Enums.BuilderPieceType.DoubleEnd, new Vector2(0, 1) }
        };


        /// <summary>
        /// This returns a in a very very special format! It is in startX, startY. All points to be multiplied by width of tile
        /// </summary>
        /// <param name="builderPieceType">The builder piece type, only some accepted.</param>
        /// <returns>Vector4 in a very very special format!</returns>
        public static Vector2 GetOriginalSpriteCuttingPoints(Enums.BuilderPieceType builderPieceType)
        {
            if (originalSpriteCuttingPoints.ContainsKey(builderPieceType))
            {
                return originalSpriteCuttingPoints[builderPieceType];
            }

            return Vector2.one;
        }
    }
}

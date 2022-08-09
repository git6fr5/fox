/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
// Platformer.
using Platformer.CustomTiles;

namespace Platformer.CustomTiles {

    ///<summary>
    /// Organizes mappings for custom tiles to grab an index from a spritesheet.
    ///<summary>
    public class CustomTileMappings {

        // The mapping for the custom ground tile.
        private static Dictionary<int, int> m_GroundTileMapping;
        public static Dictionary<int, int> GroundTileMapping => m_GroundTileMapping;

        // The mapping for the custom water tile.
        public static Dictionary<int, int> WaterTileMapping => m_WaterTileMapping;
        private static Dictionary<int, int> m_WaterTileMapping;

        // An easy reference for a list of diagonals.
        private static List<int> m_Diagonals = new List<int>() { 1, 4, 32, 128 };

        // Creates the mapping for the custom ground tile.
        public static void CreateGroundTileMapping() {
            m_GroundTileMapping = new Dictionary<int, int>();

            // Floating Block
            m_GroundTileMapping.Add(0, 0);

            // 9x9
            // Upper Row.
            m_GroundTileMapping.AddAllKeyCombinations(16 + 64 + 128, new List<int>{ 1, 4, 32 }, 1); // Left + Up
            m_GroundTileMapping.AddAllKeyCombinations(8 + 16 + 32 + 64 + 128, new List<int>{ 1, 4 }, 2); // Up
            m_GroundTileMapping.AddAllKeyCombinations(8 + 32 + 64, new List<int>{ 1, 4, 128 }, 3); // Right + up
            // Center Row.
            m_GroundTileMapping.AddAllKeyCombinations(2 + 4 + 16 + 64 + 128, new List<int>{ 1, 32 }, 4); // Left
            m_GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 32 + 64 + 128, 5); // Center
            m_GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8 + 32 + 64, new List<int>{ 4, 128 }, 6); // Right
            // Lower Row.
            m_GroundTileMapping.AddAllKeyCombinations(2 + 4 + 16, new List<int>{ 1, 32, 128 }, 7); // Down + Left
            m_GroundTileMapping.AddAllKeyCombinations(1 + 2 + 4 + 8 + 16, new List<int>{ 32, 128}, 8); // Down
            m_GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8, new List<int>{ 4, 32, 128 }, 9); // Down + Right

            // Pillars
            m_GroundTileMapping.AddAllKeyCombinations(64, m_Diagonals, 10); // Pillar Up
            m_GroundTileMapping.AddAllKeyCombinations(2 + 64, m_Diagonals, 11); // Pillar Center
            m_GroundTileMapping.AddAllKeyCombinations(2, m_Diagonals, 12); // Pillar Down

            // Platforms
            m_GroundTileMapping.AddAllKeyCombinations(16, m_Diagonals, 13);
            m_GroundTileMapping.AddAllKeyCombinations(8 + 16, m_Diagonals, 14);
            m_GroundTileMapping.AddAllKeyCombinations(8, m_Diagonals, 15);

            // 1 Corner, No Borders
            m_GroundTileMapping.Add(1 + 2 + 8 + 16 + 32 + 64 + 128, 16);
            m_GroundTileMapping.Add(2 + 4 + 8 + 16 + 32 + 64 + 128, 17);
            m_GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 32 + 64, 18);
            m_GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 64 + 128, 19);

            // 1 Corner, 1 Vertial Border
            m_GroundTileMapping.AddAllKeyCombinations(2 + 16 + 64 + 128, new List<int>{ 32 }, 20);
            m_GroundTileMapping.AddAllKeyCombinations(2 + 8 + 32 + 64, new List<int>{ 128 }, 21);
            m_GroundTileMapping.AddAllKeyCombinations(2 + 4 + 16 + 64, new List<int>{ 1 }, 22);
            m_GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8 + 64, new List<int>{ 4 }, 23);

            // 1 Corner, 2 Border
            m_GroundTileMapping.AddAllKeyCombinations(2 + 16, new List<int>{ 32, 128 }, 24);
            m_GroundTileMapping.AddAllKeyCombinations(2 + 8, new List<int>{ 32, 128 }, 25);
            m_GroundTileMapping.AddAllKeyCombinations(64 + 16, new List<int>{ 1, 4 }, 26);
            m_GroundTileMapping.AddAllKeyCombinations(64 + 8, new List<int>{ 1, 4 }, 27);

            // 2 Corner, 1 Border
            m_GroundTileMapping.AddAllKeyCombinations(2 + 16 + 64, new List<int>{ 1, 32 }, 28);
            m_GroundTileMapping.AddAllKeyCombinations(2 + 8 + 64, new List<int>{ 4, 128 }, 29);
            m_GroundTileMapping.AddAllKeyCombinations(8 + 16 + 64, new List<int>{ 1, 4 }, 30);
            m_GroundTileMapping.AddAllKeyCombinations(2 + 8 + 16, new List<int>{ 32, 128 }, 31);

            // 2 Corner, No Border
            m_GroundTileMapping.Add(2 + 8 + 16 + 32 + 64 + 128, 32);
            m_GroundTileMapping.Add(2 + 4 + 8 + 16 + 64 + 128, 33);
            m_GroundTileMapping.Add(1 + 2 + 4 + 8 + 16 + 64, 34);
            m_GroundTileMapping.Add(1 + 2 + 8 + 16 + 32 + 64, 35);

            // 1 Corner, 1 Horizontal Border
            m_GroundTileMapping.AddAllKeyCombinations(8 + 16 + 64 + 128, new List<int>{ 1, 4 }, 36);
            m_GroundTileMapping.AddAllKeyCombinations(8 + 16 + 32 + 64, new List<int>{ 1, 4 }, 37);
            m_GroundTileMapping.AddAllKeyCombinations(2 + 4 + 8 + 16, new List<int>{ 32, 128 }, 38);
            m_GroundTileMapping.AddAllKeyCombinations(1 + 2 + 8 + 16, new List<int>{ 32, 128 }, 39);

            // 3 Corners
            m_GroundTileMapping.Add(2 + 8 + 16 + 64 + 128, 40);
            m_GroundTileMapping.Add(2 + 8 + 16 + 32 + 64, 41);
            m_GroundTileMapping.Add(2 + 4 + 8 + 16 + 64, 42);
            m_GroundTileMapping.Add(1 + 2 + 8 + 16 + 64, 43);

            // 2 Corners Adjacent
            m_GroundTileMapping.Add(1 + 2 + 8 + 16 + 64 + 128, 44);
            m_GroundTileMapping.Add(2 + 4 + 8 + 16 + 32 + 64, 45);

            // All 4 Corners.
            m_GroundTileMapping.Add(2 + 8 + 16 + 64, 48);

        }

        // Creates the mapping for the custom water tile.
        public static void CreateWaterTileMapping() {
            m_WaterTileMapping = new Dictionary<int, int>();
            m_WaterTileMapping.AddAllKeyCombinations(64, new List<int>{ 1, 4, 8, 16, 32, 128 }, 0); // Surface
            m_WaterTileMapping.AddAllKeyCombinations(2 + 64, new List<int>{ 1, 4, 8, 16, 32, 128 }, 1); // Center Column
            m_WaterTileMapping.AddAllKeyCombinations(2, new List<int>{ 1, 4, 8, 16, 32, 128 }, 2); // Bottom
            m_WaterTileMapping.AddAllKeyCombinations(0, new List<int>{ 8, 16 }, 3); // Bottom
        }

        

    }


}

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
    /// Create a array of boolean integers that
    /// tracks whether the neighbouring spaces of a tile
    /// at a given position are filled.
    ///<summary>
    public class NeighbourTileArray {

        // Stores the neigbouring tile data.
        private int[] m_BitArray;
        private int[,] m_PatternArray;

        // Converts the bit array into a base 2 ID.
        public int BinaryValue => GetBinaryValue();


        // Constructs the bit array.
        public NeighbourTileArray(Vector3Int position, ITilemap tilemap) {
            // Initializing the bit array.
            m_BitArray = new int[8];
            // Cardinal tiles.
            m_BitArray[1] = tilemap.GetTile(position + Vector3Int.up) != null ? 1 : 0;
            m_BitArray[3] = tilemap.GetTile(position + Vector3Int.left) != null ? 1 : 0;
            m_BitArray[6] = tilemap.GetTile(position + Vector3Int.down) != null ? 1 : 0;
            m_BitArray[4] = tilemap.GetTile(position + Vector3Int.right) != null ? 1 : 0;
            // Diagonal tiles.
            m_BitArray[2] = tilemap.GetTile(position + Vector3Int.right + Vector3Int.up) != null ? 1 : 0;
            m_BitArray[0] = tilemap.GetTile(position + Vector3Int.left + Vector3Int.up) != null ? 1 : 0;
            m_BitArray[5] = tilemap.GetTile(position + Vector3Int.left + Vector3Int.down) != null ? 1 : 0;
            m_BitArray[7] = tilemap.GetTile(position + Vector3Int.right + Vector3Int.down) != null ? 1 : 0;

        }

        private int GetBinaryValue() {
            int val = 0;
            for (int i = 0; i < m_BitArray.Length; i++) {
                val += (int)Mathf.Pow(2, i) * m_BitArray[i];
            }
            return val;
        }

        private int[,] GetPatternArray() {

            // 0 = nothing above => line
            // 1 = line
            // 2 corner

            int[,] pattern = new int[4,4];
            
            if (m_BitArray[1] != 0) {

                for (int i = 0; i < 4; i++) {
                    pattern[i, 0] += 1;
                }

            }

            if (m_BitArray[3] != 0) {

                for (int i = 0; i < 4; i++) {
                    pattern[0, i] += 1;
                }

            }

            if (m_BitArray[6] != 0) {

                for (int i = 0; i < 4; i++) {
                    pattern[i, 3] += 1;
                }

            }

            if (m_BitArray[4] != 0) {

                for (int i = 0; i < 4; i++) {
                    pattern[3, i] += 1;
                }

            }

            return pattern;
        }

    }

}
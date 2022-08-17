/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class GroundTileEditor {

        public static Dictionary<int, int> Mapping => m_Mapping;
        private static Dictionary<int, int> m_Mapping;

        public static Dictionary<int, int> WaterMapping => m_WaterMapping;
        private static Dictionary<int, int> m_WaterMapping;

        public static void CreateMapping() {
            m_Mapping = new Dictionary<int, int>();

            // 1   2   4
            // 8   x   16
            // 32  64  128

            // 1 2 3
            // 4 5 6
            // 7 8 9

            // 2 + 3
            // 1 + 4

            // 1 + 128 => 

            // 00000000 -> sprite

            // o's
            List<int> diagonals = new List<int>() {
                1, 4, 32, 128
            };

            // Floating Block
            m_Mapping.Add(0, 0);

            // 9x9
            Add(16 + 64 + 128, new List<int>{ 1, 4, 32 }, 1, ref m_Mapping); // Left + Up
            Add(8 + 16 + 32 + 64 + 128, new List<int>{ 1, 4 }, 2, ref m_Mapping); // Up
            Add(8 + 32 + 64, new List<int>{ 1, 4, 128 }, 3, ref m_Mapping); // Right + up

            Add(2 + 4 + 16 + 64 + 128, new List<int>{ 1, 32 }, 4, ref m_Mapping); // Left
            m_Mapping.Add(1 + 2 + 4 + 8 + 16 + 32 + 64 + 128, 5); // Center
            Add(1 + 2 + 8 + 32 + 64, new List<int>{ 4, 128 }, 6, ref m_Mapping); // Right

            Add(2 + 4 + 16, new List<int>{ 1, 32, 128 }, 7, ref m_Mapping); // Down + Left
            Add(1 + 2 + 4 + 8 + 16, new List<int>{ 32, 128}, 8, ref m_Mapping); // Down
            Add(1 + 2 + 8, new List<int>{ 4, 32, 128 }, 9, ref m_Mapping); // Down + Right

            // Pillar
            Add(64, diagonals, 10, ref m_Mapping); // Pillar Up
            Add(2 + 64, diagonals, 11, ref m_Mapping); // Pillar Center
            Add(2, diagonals, 12, ref m_Mapping); // Pillar Down

            // Platform
            Add(16, diagonals, 13, ref m_Mapping);
            Add(8 + 16, diagonals, 14, ref m_Mapping);
            Add(8, diagonals, 15, ref m_Mapping);

            // 1 Corner No Borders
            m_Mapping.Add(1 + 2 + 8 + 16 + 32 + 64 + 128, 16);
            m_Mapping.Add(2 + 4 + 8 + 16 + 32 + 64 + 128, 17);
            m_Mapping.Add(1 + 2 + 4 + 8 + 16 + 32 + 64, 18);
            m_Mapping.Add(1 + 2 + 4 + 8 + 16 + 64 + 128, 19);

            // 1 Corner 1 Vertial Border
            Add(2 + 16 + 64 + 128, new List<int>{ 32 }, 20, ref m_Mapping);
            Add(2 + 8 + 32 + 64, new List<int>{ 128 }, 21, ref m_Mapping);
            Add(2 + 4 + 16 + 64, new List<int>{ 1 }, 22, ref m_Mapping);
            Add(1 + 2 + 8 + 64, new List<int>{ 4 }, 23, ref m_Mapping);

            // 1 Corner 2 Border
            Add(2 + 16, new List<int>{ 32, 128 }, 24, ref m_Mapping);
            Add(2 + 8, new List<int>{ 32, 128 }, 25, ref m_Mapping);
            Add(64 + 16, new List<int>{ 1, 4 }, 26, ref m_Mapping);
            Add(64 + 8, new List<int>{ 1, 4 }, 27, ref m_Mapping);

            // 2 Corner 1 Border
            Add(2 + 16 + 64, new List<int>{ 1, 32 }, 28, ref m_Mapping);
            Add(2 + 8 + 64, new List<int>{ 4, 128 }, 29, ref m_Mapping);
            Add(8 + 16 + 64, new List<int>{ 1, 4 }, 30, ref m_Mapping);
            Add(2 + 8 + 16, new List<int>{ 32, 128 }, 31, ref m_Mapping);

            // 2 Corner No Border
            m_Mapping.Add(2 + 8 + 16 + 32 + 64 + 128, 32);
            m_Mapping.Add(2 + 4 + 8 + 16 + 64 + 128, 33);
            m_Mapping.Add(1 + 2 + 4 + 8 + 16 + 64, 34);
            m_Mapping.Add(1 + 2 + 8 + 16 + 32 + 64, 35);

            // 1 Corner 1 Horizontal Border
            Add(8 + 16 + 64 + 128, new List<int>{ 1, 4 }, 36, ref m_Mapping);
            Add(8 + 16 + 32 + 64, new List<int>{ 1, 4 }, 37, ref m_Mapping);
            Add(2 + 4 + 8 + 16, new List<int>{ 32, 128 }, 38, ref m_Mapping);
            Add(1 + 2 + 8 + 16, new List<int>{ 32, 128 }, 39, ref m_Mapping);

            // 3 Corners
            m_Mapping.Add(2 + 8 + 16 + 64 + 128, 40);
            m_Mapping.Add(2 + 8 + 16 + 32 + 64, 41);
            m_Mapping.Add(2 + 4 + 8 + 16 + 64, 42);
            m_Mapping.Add(1 + 2 + 8 + 16 + 64, 43);

            // 2 Corners Adjacent
            m_Mapping.Add(1 + 2 + 8 + 16 + 64 + 128, 44);
            m_Mapping.Add(2 + 4 + 8 + 16 + 32 + 64, 45);
            // m_Mapping.Add(2 + 4 + 8 + 16 + 32 + 64, 42);
            // m_Mapping.Add(1 + 2 + 8 + 16 + 64 + 128, 43);

            // All 4 Corners.
            m_Mapping.Add(2 + 8 + 16 + 64, 48);

            /* ---------------------------- */

            // 1   2   4
            // 8   x   16
            // 32  64  128

            m_WaterMapping = new Dictionary<int, int>();
            Add(64, new List<int>{ 1, 4, 8, 16, 32, 128 }, 0, ref m_WaterMapping); // Surface
            Add(2 + 64, new List<int>{ 1, 4, 8, 16, 32, 128 }, 1, ref m_WaterMapping); // Center Column
            Add(2, new List<int>{ 1, 4, 8, 16, 32, 128 }, 2, ref m_WaterMapping); // Bottom
            Add(0, new List<int>{ 8, 16 }, 3, ref m_WaterMapping); // Bottom
        }

        public static void Add(int x, List<int> o, int y, ref Dictionary<int, int> dict) {

            List<int> t = new List<int>();
            t.Add(0);

            int[] l = new int[o.Count];
            for (int i = 0; i < o.Count; i++) {
                l[i] = 0;
            }
            
            for (int n = 0; n < (int)Mathf.Pow(2, o.Count) - 1; n++) {

                // next.
                l[0] += 1;
                for (int i = 0; i < l.Length; i++) {
                    if (l[i] > 1) {
                        l[i] = 0;
                        l[i + 1] += 1;
                    }
                }

                // calc.
                int val = 0;
                for (int i = 0; i < o.Count; i++) {
                    if (l[i] == 1) {
                        val += o[i];
                    }
                }

                // append.
                t.Add(val);

            }

            for (int i = 0; i < t.Count; i++) {
                dict.Add(x + t[i], y);
            }
        }

    }


}

/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.CustomTiles;

namespace Platformer.CustomTiles {

    ///<summary>
    /// Defines useful extension for manipulating dictionary in
    /// the context of custom tiles.
    ///<summary>
    public static class DictionaryExtensions {

        // Appends all combination of kv pairs with k as a base and itterating through the combinations
        // of c. With v as the value. 
        public static void AddAllKeyCombinations(this Dictionary<int, int> dict, int k, List<int> c, int v) {
            List<int> t = new List<int>();
            t.Add(0);

            int[] l = new int[c.Count];
            for (int i = 0; i < c.Count; i++) {
                l[i] = 0;
            }
            
            for (int n = 0; n < (int)Mathf.Pow(2, c.Count) - 1; n++) {

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
                for (int i = 0; i < c.Count; i++) {
                    if (l[i] == 1) {
                        val += c[i];
                    }
                }

                // append.
                t.Add(val);

            }

            for (int i = 0; i < t.Count; i++) {
                dict.Add(k + t[i], v);
            }
        }

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Utilities {

        public static bool CycleIndexIf(ref int index, int value, int length, bool predicate) {
            bool wasnotzero = index != 0;
            if (predicate) { 
                index = (index + value) % length; 
            }
            bool isnowzero = index == 0;
            return wasnotzero && isnowzero;
        }

        public static int RandomIndex(int length) {
            return UnityEngine.Random.Range(0, length); 
        }

    }

}
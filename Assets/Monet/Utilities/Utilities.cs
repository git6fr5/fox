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

        public static void CycleIndexIf(ref int index, int value, int length, bool predicate) {
            if (predicate) { 
                index = (index + value) % length; 
            }
        }

        public static int RandomIndex(int length) {
            return UnityEngine.Random.Range(0, length); 
        }

    }

}
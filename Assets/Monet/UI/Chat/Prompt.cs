/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class Prompt {

        // The interval between printing letters.
        public static float PrintInterval = 0.035f; 

        // The lines this prompt has.
        [SerializeField] private string[] m_Lines;
        public string[] Lines => m_Lines;

    }

}
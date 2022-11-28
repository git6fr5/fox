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
    /// A simple array of sprites for animating custom tiles.
    /// Yes. This is equivalent to using a 2D array.
    /// But doing it like this is just easier to read.
    ///<summary>
    [System.Serializable]
    public class TileAnimation {
        [SerializeField] private Sprite[] m_Sprites;
        public Sprite[] Sprites => m_Sprites;
    }

}
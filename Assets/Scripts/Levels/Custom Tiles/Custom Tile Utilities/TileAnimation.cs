/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.CustomTiles;

/* --- Definitions --- */
using Game = Platformer.GameManager;

namespace Platformer.CustomTiles {

    ///<summary>
    /// A simple array of sprites for animating custom tiles.
    /// Yes. This is equivalent to using a 2D array.
    /// But doing it like this is just easier to read.
    ///<summary>
    [System.Serializable]
    public class TileAnimation {

        // The rate at which this animation plays.
        protected const float FRAME_RATE = 4;

        // The sprites used for animating this tile.
        [SerializeField] 
        private Sprite[] m_Sprites;
        public Sprite[] Sprites => m_Sprites;

        // Returns the current frame of this animation.
        public Sprite CurrentFrame => GetFrame(Game.Physics.Ticks);

        // Gets the frame based on the given ticks.
        public Sprite GetFrame(float ticks) {
            int frame = (int)Mathf.Floor(ticks * FRAME_RATE) % m_Sprites.Length;
            return m_Sprites[frame];
        }

    }

}
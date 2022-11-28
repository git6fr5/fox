/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.LevelLoader;

namespace Platformer.LevelLoader {

    ///<summary>
    /// A useful data structure for reading and matching LDtk data to game data.
    ///<summary>
    public class LDtkTileData {

        public static Vector2Int ControlStopID = new Vector2Int(1, 0);
        
        private Vector2Int m_VectorID;
        public Vector2Int VectorID => m_VectorID;

        private Vector2Int m_GridPosition;
        public Vector2Int GridPosition => m_GridPosition;

        private int m_Index;
        public int Index => m_Index;

        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            m_VectorID = vectorID;
            m_GridPosition = gridPosition;
            m_Index = index;
        }

    }

}
    
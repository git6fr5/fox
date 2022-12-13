/* --- Libraries --- */
// System.
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
// Platformer.
using Platformer.Rendering;
using Platformer.CustomTiles;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platformer.CustomTiles {

    /// <summary>
    /// A custom tile that defines an automatically tiling
    /// ground tile.
    /// <summary>
    [Serializable]
    public class GroundTile : Tile {

        #region Fields

        /* --- Member Variables --- */

        // A list of the sprites in the tileset.
        [SerializeField] 
        public Sprite[] m_Sprites;

        // A reference to the Ground Tile mapping for easy reference.
        private Dictionary<int, int> m_Mapping => CustomTileMappings.GroundTileMapping;

        #endregion

        // Gets the data and sets the sprite.
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            // Guard clause.
            if (m_Mapping == null || m_Sprites == null || m_Sprites.Length == 0) {
                return;
            }

            // Get the sprite based on the neighbour data.
            int neighbours = new NeighbourTileArray(position, tilemap).BinaryValue;
            int index = m_Mapping.ContainsKey(neighbours) ? m_Mapping[neighbours] : 0;
            tileData.sprite = m_Sprites[index];
            tileData.colliderType = Tile.ColliderType.Grid;
        }

        #if UNITY_EDITOR
        // Creates the tile asset.
        [MenuItem("Assets/Create/2D/Custom Tiles/GroundTile")]
        public static void CreateGroundTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save GroundTile", "New GroundTile", "Asset", "Save GroundTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GroundTile>(), path);
        }
        #endif


    }

}

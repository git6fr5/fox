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
using Platformer.CustomTiles;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platformer.CustomTiles {

    /// <summary>
    /// A custom tile that defines an automatically tiling
    /// water tile.
    /// <summary>
    [Serializable]
    public class WaterTile : Tile {

        #region Fields.

        /* --- Constants --- */

        // The opacity of the water.
        private const float OPACITY = 0.75f;

        /* --- Member Variables --- */

        // A list of the animations in the tileset.
        [SerializeField] 
        public TileAnimation[] m_Animations;

        // A reference to the Water Tile mapping for easy reference.
        private Dictionary<int, int> m_Mapping => CustomTileMappings.WaterTileMapping;

        #endregion

        #region Methods.

        // Gets the data and sets the sprite.
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            int neighbours = new NeighbourTileArray(position, tilemap).BinaryValue;

            // If there is an index this is mapped to then animate the tile.
            if (m_Mapping != null && m_Mapping.ContainsKey(neighbours)) {
                int animationIndex = m_Mapping[neighbours];
                tileData.sprite = m_Animations[animationIndex].CurrentFrame;
            }
            else if (m_Animations != null && m_Animations.Length > 0 && m_Animations[0].Sprites.Length > 0) {
                tileData.sprite = m_Animations[0].Sprites[0];
            }
            else {
                tileData.sprite = null;
            }

            // The opacity and collision type.
            tileData.color = new Color(1f, 1f, 1f, OPACITY);
            tileData.colliderType = Tile.ColliderType.Grid;

        }

        #if UNITY_EDITOR
        // Creates the tile asset.
        [MenuItem("Assets/Create/2D/Custom Tiles/WaterTile")]
        public static void CreateWaterTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save WaterTile", "New WaterTile", "Asset", "Save WaterTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WaterTile>(), path);
        }
        #endif

        #endregion

    }

}


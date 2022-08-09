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
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

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

        // A list of the animations in the tileset.
        [SerializeField] public TileAnimation[] m_Animations;

        // A reference to the Water Tile mapping for easy reference.
        private Dictionary<int, int> NeighbourToIndex => CustomTileMappings.WaterTileMapping;

        // The opacity of the water.
        [SerializeField] private float m_Opacity = 0.75f;

        // Gets the data and sets the sprite.
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            int neighbours = new NeighbourTileArray(position, tilemap).BinaryValue;
            if (NeighbourToIndex != null && NeighbourToIndex.ContainsKey(neighbours)) {
                // Animate the tile.
                int index = NeighbourToIndex[neighbours];
                TileAnimation animation = m_Animations[index];
                int frame = (int)Mathf.Floor(Game.Ticks * Screen.FrameRate) % animation.Sprites.Length;
                tileData.sprite = animation.Sprites[frame];
            }
            else if (m_Animations != null && m_Animations.Length > 0 && m_Animations[0].Sprites.Length > 0) {
                tileData.sprite = m_Animations[0].Sprites[0];
            }
            else {
                tileData.sprite = null;
            }
            tileData.color = new Color(1f, 1f, 1f, m_Opacity);
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

    }

}


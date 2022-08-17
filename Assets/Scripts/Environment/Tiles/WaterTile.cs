using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
using Monet;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Definitions.
using Neighbours = Monet.GroundTile.Neighbours;

namespace Monet {

    [Serializable]
    public class WaterTile : Tile {

        [System.Serializable]
        public class TileAnimation {
            [SerializeField] private Sprite[] m_Sprites;
            public Sprite[] Sprites => m_Sprites;
        }

        [SerializeField] public TileAnimation[] m_Animations;

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {

            Neighbours neighbours = new Neighbours(position, tilemap);
            if (GroundTileEditor.WaterMapping != null && GroundTileEditor.WaterMapping.ContainsKey(neighbours.BinaryValue)) {

                TileAnimation animation = m_Animations[GroundTileEditor.WaterMapping[neighbours.BinaryValue]];
                int frame = (int)Mathf.Floor(Game.Ticks * Screen.FrameRate) % animation.Sprites.Length;
                tileData.sprite = animation.Sprites[frame];

            }
            else if (m_Animations != null && m_Animations.Length > 0 && m_Animations[0].Sprites.Length > 0) {
                tileData.sprite = m_Animations[0].Sprites[0];
            }
            else {
                tileData.sprite = null;
            }
            
            tileData.color = new Color(1f, 1f, 1f, 0.75f);
            tileData.colliderType = Tile.ColliderType.Grid;
        }

        #if UNITY_EDITOR
        [MenuItem("Assets/Create/2D/Custom Tiles/WaterTile")]
        public static void CreateWaterTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save WaterTile", "New WaterTile", "Asset", "Save WaterTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WaterTile>(), path);
        }
        #endif

    }

}


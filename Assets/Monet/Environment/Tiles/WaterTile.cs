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

        [SerializeField] public Sprite[] m_Sprites;

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            Neighbours neighbours = new Neighbours(position, tilemap);
            if (GroundTileEditor.WaterMapping != null && GroundTileEditor.WaterMapping.ContainsKey(neighbours.BinaryValue)) {
                tileData.sprite = m_Sprites[GroundTileEditor.WaterMapping[neighbours.BinaryValue]];
            }
            else if (m_Sprites != null && m_Sprites.Length > 0) {
                tileData.sprite = m_Sprites[0];
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


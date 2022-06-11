using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Tilesheet : Tile {

    [SerializeField] public Sprite[] m_Sprites;

    public class Neighbours {
        public int BinaryValue => GetBinaryValue();
        private int[] m_BitArray;

        public Neighbours(Vector3Int position, ITilemap tilemap) {

            m_BitArray = new int[8];

            m_BitArray[0] = tilemap.GetTile(position + Vector3Int.left + Vector3Int.up) != null ? 1 : 0;
            m_BitArray[1] = tilemap.GetTile(position + Vector3Int.up) != null ? 1 : 0;
            m_BitArray[2] = tilemap.GetTile(position + Vector3Int.right + Vector3Int.up) != null ? 1 : 0;

            m_BitArray[3] = tilemap.GetTile(position + Vector3Int.left) != null ? 1 : 0;
            m_BitArray[4] = tilemap.GetTile(position + Vector3Int.right) != null ? 1 : 0;

            m_BitArray[5] = tilemap.GetTile(position + Vector3Int.left + Vector3Int.down) != null ? 1 : 0;
            m_BitArray[6] = tilemap.GetTile(position + Vector3Int.down) != null ? 1 : 0;
            m_BitArray[7] = tilemap.GetTile(position + Vector3Int.right + Vector3Int.down) != null ? 1 : 0;

        }

        private int GetBinaryValue() {
            int val = 0;
            for (int i = 0; i < m_BitArray.Length; i++) {
                val += (int)Mathf.Pow(2, i) * m_BitArray[i];
            }
            return val;
        }

    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
        for (int i = -1; i < 1; i++) {
            for (int j = -1; j < 1; j++) {
                base.RefreshTile(position + new Vector3Int(j, i, 0), tilemap);
            }
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        Neighbours neighbours = new Neighbours(position, tilemap);
        if (TilemapEditor.Mapping != null && TilemapEditor.Mapping.ContainsKey(neighbours.BinaryValue)) {
            tileData.sprite = m_Sprites[TilemapEditor.Mapping[neighbours.BinaryValue]];
        }
        else if (m_Sprites != null && m_Sprites.Length > 0) {
            tileData.sprite = m_Sprites[0];
        }
        else {
            tileData.sprite = null;
        }
        tileData.colliderType = Tile.ColliderType.Grid;
        // Console.Log(neighbours.BinaryValue);
    }

    #if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom Tiles/Tilesheet")]
    public static void CreateTilesheet() {
        string path = EditorUtility.SaveFilePanelInProject("Save Tilesheet", "New Tilesheet", "Asset", "Save Tilesheet", "Assets");
        if (path == "") { return; }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Tilesheet>(), path);
    }
    #endif

}

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
public class Watersheet : Tile {

    public Sprite[] Variations;
    private static Dictionary<Vector3Int, int> m_IndexDict = new Dictionary<Vector3Int, int>();

    public static void SetVariation(Vector3Int position, int index) {
        if (!m_IndexDict.ContainsKey(position)) {
            m_IndexDict.Add(position, index);
        }
        else {
            m_IndexDict[position] = index;
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = Variations[m_IndexDict[position] % Variations.Length];
        tileData.color = new Color(1f, 1f, 1f, 0.75f);
        tileData.colliderType = Tile.ColliderType.Grid;
    }

    #if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom Tiles/Watersheet")]
    public static void CreateWatersheet() {
        string path = EditorUtility.SaveFilePanelInProject("Save Watersheet", "New Watersheet", "Asset", "Save Watersheet", "Assets");
        if (path == "") { return; }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Watersheet>(), path);
    }
    #endif

}
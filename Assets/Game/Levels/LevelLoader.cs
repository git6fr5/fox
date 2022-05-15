/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;
using LDtkLayer = LDtkUnity.LayerInstance;
using LDtkTile = LDtkUnity.TileInstance;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Level))]
[RequireComponent(typeof(Environment))]
public class LevelLoader : MonoBehaviour {

    /* --- Data --- */
    #region  Data

    public class LDtkTileData {
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public int index;
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
        }

    }

    #endregion

    /* --- Variables --- */
    #region Variables

    // Layer Names
    public static string CharacterLayer = "Characters";
    public static string GroundLayer = "Ground";

    // Grid Size
    public static int GridSize = 16;

    // Components.
    [SerializeField] private LDtkComponentProject m_LDtkData;
    [HideInInspector] private Environment m_Environment;
    [HideInInspector] private Level m_CurrLevel;
    [HideInInspector] private LdtkJson m_JSON;

    #endregion

    /* --- Unity --- */
    #region Unity
    
    void Start() {
        m_JSON = m_LDtkData.FromJson();
        m_Environment = GetComponent<Environment>();
        m_Environment.Init();
        m_CurrLevel = GetComponent<Level>();
        OpenLevel(m_CurrLevel.LevelName);
    }
    
    #endregion

    /* --- File Loading --- */
    #region File

    public void OpenLevel(string levelName) {
        LDtkLevel ldtkLevel = GetLevel(levelName);
        OpenLevel(ldtkLevel);
    }

    protected LDtkLevel GetLevel(string levelName) {
        for (int i = 0; i < m_JSON.Levels.Length; i++) {
            if (m_JSON.Levels[i].Identifier == levelName) {
                Debug.Log("Found Level");
                return m_JSON.Levels[i];
            }
        }
        return null;
    }

    protected void OpenLevel(LDtkLevel ldtkLevel) {
        if (ldtkLevel != null) {
            // Load the data.
            List<LDtkTileData> characterData = LoadLayer(ldtkLevel, CharacterLayer);
            List<LDtkTileData> groundData = LoadLayer(ldtkLevel, GroundLayer);
            // Load the level.
            m_CurrLevel.GenerateEntities(characterData, m_Environment.Characters);
            m_CurrLevel.GenerateTiles(groundData, m_Environment.Tile);
        }
    }

    #endregion

    /* --- Layer Loading --- */
    #region Layer
    
    private List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        List<LDtkTileData> layerData = new List<LDtkTileData>();
        LDtkLayer layer = GetLayer(ldtkLevel, layerName);
        if (layer == null) { return layerData; }

        for (int index = 0; index < layer.GridTiles.Length; index++) {
            LDtkTile tile = layer.GridTiles[index];
            layerData.Add(new LDtkTileData(new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / GridSize, tile.UnityPx / GridSize, index));
        }
        return layerData;
    }

    
    private LDtkUnity.LayerInstance GetLayer(LDtkLevel ldtkLevel, string layerName) {
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkLayer layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    private Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
        for (int i = 0; i < data.Count; i++) {
            if (gridPosition == data[i].gridPosition) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }
    
    #endregion



}

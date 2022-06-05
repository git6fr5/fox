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
    public static string ControlLayer = "Controls";
    public static string EntityLayer = "Entities";
    public static string GroundLayer = "Ground";
    public static string BackgroundLayer = "Background";

    // Grid Size
    public static int GridSize = 16;

    // Components.
    [SerializeField] protected LDtkComponentProject m_LDtkData;
    [HideInInspector] protected Environment m_Environment;
    [HideInInspector] protected LdtkJson m_JSON;

    #endregion

    /* --- Unity --- */
    #region Unity
    
    protected virtual void Start() {
        m_JSON = m_LDtkData.FromJson();
        m_Environment = GetComponent<Environment>();
        m_Environment.Init();
        // m_CurrLevel = GetComponent<Level>();
        // OpenLevel(m_CurrLevel.LevelName);
    }
    
    #endregion

    /* --- File Loading --- */
    #region File

    public void OpenLevel(Level level) {
        LDtkLevel ldtkLevel = level.LDtkLevel; // GetLevel(level.LevelName);
        LoadLevel(level, ldtkLevel);
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

    protected void LoadLevel(Level level, LDtkLevel ldtkLevel) {
        if (ldtkLevel != null) {
            // Load the data.
            List<LDtkTileData> entityData = LoadLayer(ldtkLevel, EntityLayer);
            List<LDtkTileData> groundData = LoadLayer(ldtkLevel, GroundLayer);
            List<LDtkTileData> backgroundData = LoadLayer(ldtkLevel, BackgroundLayer);
            List<LDtkTileData> controlData = LoadLayer(ldtkLevel, ControlLayer);
            // Load the level.
            level.GenerateEntities(entityData, m_Environment.Entities);
            level.GenerateTiles(groundData, m_Environment.Tile);
            level.SetBackground(backgroundData, m_Environment.BackgroundTile);
            level.SetControls(controlData, m_Environment);
        }
    }

    protected void Unload(Level level) {
        level.DestroyEntities();
        level.ClearMap();
    }

    #endregion

    /* --- Layer Loading --- */
    #region Layer
    
    protected List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        List<LDtkTileData> layerData = new List<LDtkTileData>();
        LDtkLayer layer = GetLayer(ldtkLevel, layerName);
        if (layer == null) { return layerData; }

        for (int index = 0; index < layer.GridTiles.Length; index++) {
            LDtkTile tile = layer.GridTiles[index];
            layerData.Add(new LDtkTileData(new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / GridSize, tile.UnityPx / GridSize, index));
        }
        return layerData;
    }

    
    protected LDtkUnity.LayerInstance GetLayer(LDtkLevel ldtkLevel, string layerName) {
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkLayer layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    protected Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
        for (int i = 0; i < data.Count; i++) {
            if (gridPosition == data[i].gridPosition) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }
    
    #endregion



}

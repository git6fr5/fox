using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

public class WorldLoader : LevelLoader {

    public static float BoundLimit = 70f;

    public string startingLevelName = "The_Beginning";

    public static Vector2Int LoadPointID = new Vector2Int(0, 0);

    public Level[] m_Levels;
    public List<Level> loadLevels = new List<Level>();
    public List<Level> loadedLevels = new List<Level>();
    public List<Level> deloadLevels = new List<Level>();

    [SerializeField] private Tilemap m_Ground;

    public static WorldLoader instance;

    protected override void Start() {
        base.Start();

        instance = this;

        m_Ground = new GameObject("Map", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
        m_Ground.GetComponent<TilemapRenderer>().sortingLayerName = GameRules.BorderRenderingLayer;
        m_Ground.transform.SetParent(transform);
        m_Ground.transform.localPosition = Vector3.zero;
        m_Ground.gameObject.layer = LayerMask.NameToLayer("Ground");

        CollectLevels();
        LoadAllTiles();
        SetPlayer();
        LoadLevels();
        StartCoroutine(IELoadLevels());
    }

    private void CollectLevels() {

        // Get the json file from the LDtk Data.
        List<Level> L_Levels = new List<Level>();

        for (int i = 0; i < m_JSON.Levels.Length; i++) {
            Level level = new GameObject(m_JSON.Levels[i].Identifier, typeof(Level)).GetComponent<Level>();
            level.transform.SetParent(transform);
            level.Init(i, m_JSON, m_Ground);

            List<LDtkTileData> controlData = LoadLayer(m_JSON.Levels[i], ControlLayer);
            List<Vector2Int> controlPositions = new List<Vector2Int>();
            for (int j = 0; j < controlData.Count; j++) {
                if (controlData[j].vectorID == LoadPointID) {
                    controlPositions.Add(controlData[j].gridPosition);
                }
            }
            level.SetControlPositions(controlPositions);

            // Store the data into an array.
            L_Levels.Add(level);
        }

        m_Levels = L_Levels.ToArray();

    }

    private void SetPlayer() {

        if (WorldTransition.LevelName != "") {
            startingLevelName = WorldTransition.LevelName;
        }

        for (int i = 0; i < m_Levels.Length; i++) {
            if (startingLevelName == m_Levels[i].LevelName && m_Levels[i].ControlPositions != null && m_Levels[i].ControlPositions.Count > 0) {
                GameRules.MainPlayer.transform.position = m_Levels[i].GridToWorldPosition(m_Levels[i].ControlPositions[0]);
                GameRules.MainPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                
                OpenLevel(m_Levels[i]);
                loadedLevels.Add(m_Levels[i]);
                
                break;
            }
        }

    }

    private IEnumerator IELoadLevels() {
        while (true) {
            LoadLevels();
            yield return new WaitForSeconds(0.25f);
        }
    }

    public static void Reload() {
        instance.LoadLevels();
    }

    public void LoadLevels() {

        Player player = GameRules.MainPlayer;
        loadLevels = new List<Level>();
        deloadLevels = new List<Level>();

        for (int i = 0; i < m_Levels.Length; i++) {
            Level level = m_Levels[i];
            for (int j = 0; j < level.ControlPositions.Count; j++) {
                // newBanana.transform.position = newLevel.GridToWorldPosition(newLevel.controlPosition + new Vector2Int(0, -1));
                Vector3 position = level.GridToWorldPosition(level.ControlPositions[j] + new Vector2Int(0, -1));
                float dist = (position - player.transform.position).magnitude;
                if (dist < BoundLimit) {
                    Debug.DrawLine(player.transform.position, position, new Color(Mathf.Min(BoundLimit, dist) / BoundLimit, 0f, 1f - Mathf.Min(BoundLimit, dist) / BoundLimit), 0.25f);
                }
                if (!loadLevels.Contains(level) && (position - player.transform.position).sqrMagnitude < BoundLimit * BoundLimit) {
                    loadLevels.Add(level);
                }
            }
        }

        for (int i = 0; i < loadLevels.Count; i++) {
            if (!loadedLevels.Contains(loadLevels[i])) {
                OpenLevel(loadLevels[i]);
                loadedLevels.Add(loadLevels[i]);
            }
        }

        for (int i = 0; i < loadedLevels.Count; i++) {
            if (!loadLevels.Contains(loadedLevels[i])) {
                deloadLevels.Add(loadedLevels[i]);
            }
        }

        for (int i = 0; i < deloadLevels.Count; i++) {
            Unload(deloadLevels[i]);
            loadedLevels.Remove(deloadLevels[i]);
        }

    }

    public void LoadAllTiles() {
        for (int i = 0; i < m_Levels.Length; i++) {

            LDtkLevel ldtkLevel = m_Levels[i].LDtkLevel; // GetLevel(level.LevelName);
            List<LDtkTileData> groundData = LoadLayer(ldtkLevel, GroundLayer);
            m_Levels[i].GenerateTiles(groundData, m_Environment.Tile);

        }
        m_Ground.RefreshAllTiles();
    }

}

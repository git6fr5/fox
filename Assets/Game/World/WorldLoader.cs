using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

public class WorldLoader : LevelLoader {

    public static float BoundLimit = 100f;

    public string startingLevelName = "The_Beginning";

    public static Vector2Int LoadPointID = new Vector2Int(0, 0);

    public Level[] m_Levels;
    public List<Level> loadLevels = new List<Level>();
    public List<Level> loadedLevels = new List<Level>();
    public List<Level> deloadLevels = new List<Level>();

    protected override void Start() {
        base.Start();
        CollectLevels();
        LoadLevels();
        StartCoroutine(IESetPlayer());
        StartCoroutine(IELoadLevels());
    }

    private void CollectLevels() {

        // Get the json file from the LDtk Data.
        List<Level> L_Levels = new List<Level>();

        for (int i = 0; i < m_JSON.Levels.Length; i++) {
            Level level = new GameObject(m_JSON.Levels[i].Identifier, typeof(Level)).GetComponent<Level>();
            level.transform.SetParent(transform);
            level.Init(i, m_JSON);

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

    private IEnumerator IESetPlayer() {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < m_Levels.Length; i++) {
            if (startingLevelName == m_Levels[i].LevelName && m_Levels[i].ControlPositions != null && m_Levels[i].ControlPositions.Count > 0) {
                GameRules.MainPlayer.transform.position = m_Levels[i].GridToWorldPosition(m_Levels[i].ControlPositions[0]);
                GameRules.MainPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                break;
            }
        }
        yield return null;
    }

    private IEnumerator IELoadLevels() {
        while (true) {
            LoadLevels();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void LoadLevels() {

        print("Loading Levels");

        Player player = GameRules.MainPlayer;
        loadLevels = new List<Level>();
        deloadLevels = new List<Level>();

        for (int i = 0; i < m_Levels.Length; i++) {
            Level level = m_Levels[i];
            for (int j = 0; j < level.ControlPositions.Count; j++) {
                // newBanana.transform.position = newLevel.GridToWorldPosition(newLevel.controlPosition + new Vector2Int(0, -1));
                Vector3 position = level.GridToWorldPosition(level.ControlPositions[j] + new Vector2Int(0, -1));
                Debug.DrawLine(player.transform.position, position, Color.red, 0.25f);
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

}

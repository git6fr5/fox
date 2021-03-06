/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using LDtkUnity;

/* --- Definitions --- */
using LDtkTileData = LevelLoader.LDtkTileData;

/// <summary>
///
/// <summary>
public class Level : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Settings.
    [SerializeField, ReadOnly] private int m_ID;
    public int ID => m_ID;
    [SerializeField, ReadOnly] private string m_LevelName;
    public string LevelName => m_LevelName;
    [SerializeField, ReadOnly] private LDtkUnity.Level m_LDtkLevel;
    public LDtkUnity.Level LDtkLevel => m_LDtkLevel;
    [SerializeField, ReadOnly] private int m_Height;
    public int Height => m_Height;
    [SerializeField, ReadOnly] private int m_Width;
    public int Width => m_Width;
    [SerializeField, ReadOnly] private int m_WorldHeight;
    public int WorldHeight => m_WorldHeight;
    [SerializeField, ReadOnly] private int m_WorldWidth;
    public int WorldWidth => m_WorldWidth;
    public Vector2Int GridOrigin => new Vector2Int(m_WorldWidth, m_WorldHeight);
    [SerializeField] private List<Vector2Int> m_ControlPositions;
    public List<Vector2Int> ControlPositions => m_ControlPositions;

    // Components.
    [SerializeField] private BoxCollider2D m_Box;
    [SerializeField] private Tilemap m_Ground;
    [SerializeField] private Tilemap m_Background;
    [SerializeField] private List<Entity> m_Entities = new List<Entity>();

    // Position.
    public Vector2 Center => (Vector2)GridToWorldPosition(new Vector2Int(m_Width / 2, m_Height / 2)) - new Vector2( m_Width % 2 == 0 ? 0.5f : 0f, m_Height % 2 == 1 ? 0f : -0.5f);

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Init(int jsonID, LdtkJson json) {
        transform.localPosition = Vector3.zero;

        m_Ground = new GameObject("Map", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
        m_Ground.GetComponent<TilemapRenderer>().sortingLayerName = GameRules.BorderRenderingLayer;
        m_Ground.transform.SetParent(transform);
        m_Ground.transform.localPosition = Vector3.zero;
        m_Ground.gameObject.layer = LayerMask.NameToLayer("Ground");;

        m_Background = new GameObject("Background", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
        m_Background.GetComponent<TilemapRenderer>().sortingLayerName = GameRules.BackgroundRenderingLayer;
        m_Background.transform.SetParent(transform);
        m_Background.transform.localPosition = Vector3.zero;

        m_ID = jsonID;
        m_LDtkLevel = json.Levels[jsonID];
        m_LevelName = json.Levels[jsonID].Identifier;
        m_Height = (int)(json.Levels[jsonID].PxHei / json.DefaultGridSize);
        m_Width = (int)(json.Levels[jsonID].PxWid / json.DefaultGridSize);

        m_WorldHeight = (int)(json.Levels[jsonID].WorldY / json.DefaultGridSize);
        m_WorldWidth = (int)(json.Levels[jsonID].WorldX / json.DefaultGridSize);


        m_Box = gameObject.AddComponent<BoxCollider2D>();
        m_Box.isTrigger = true;
        m_Box.size = new Vector2((float)(m_Width - 2), (float)(m_Height - 2));
        m_Box.offset = Center;

    }

    public void SetControlPositions(List<Vector2Int> controlPositions) {
        m_ControlPositions = controlPositions;
    }

    #endregion

    void OnTriggerEnter2D(Collider2D collider) {
        Player temp = collider.GetComponent<Player>();
        ProcessCollision(temp);
    }
    
    void OnTriggerExit2D(Collider2D collider) {
        Player temp = collider.GetComponent<Player>();
    }

    void ProcessCollision(Player player) {
        if (player == null) {
            return;
        }
        player.SetLevel(this);
    }

    /* --- Generation --- */
    #region Generation
    
    public void GenerateEntities(List<LDtkTileData> entityData, List<Entity> entityList) {
        for (int i = 0; i < entityData.Count; i++) {
            Entity entityBase = Environment.GetEntityByVectorID(entityData[i].vectorID, entityList);
            if (entityBase != null) {
                Entity newEntity = Instantiate(entityBase.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Entity>();
                Vector3 entityPosition = GridToWorldPosition(entityData[i].gridPosition);
                newEntity.Init(entityData[i].gridPosition, entityPosition);
                m_Entities.Add(newEntity);
            }
        }
    }

    public void DestroyEntities() {
        if (m_Entities != null && m_Entities.Count > 0) {
            for (int i = 0; i < m_Entities.Count; i++) {
                if (m_Entities[i] != null) {
                    Destroy(m_Entities[i].gameObject);
                }
            }
        }
    }

    public void GenerateTiles(List<LDtkTileData> tileData, RuleTile tile) {
        for (int i = 0; i < tileData.Count; i++) {
            Vector3Int tilePosition = GridToTilePosition(tileData[i].gridPosition);
            m_Ground.SetTile(tilePosition, tile);
        }
    }

    public void ClearMap() {
        m_Ground.ClearAllTiles();
    }

    public void SetBackground(RuleTile tile) {
        for (int i = 0; i < m_Height; i++) {
            for (int j = 0; j < m_Width; j++) {
                Vector3Int tilePosition = GridToTilePosition(new Vector2Int(j, i));
                m_Background.SetTile(tilePosition, tile);
            }
        }
    }

    public void SetControls(List<LDtkTileData> controlData, Environment environment) {
        for (int i = 0; i < controlData.Count; i++) {

            // Circles.
            if (controlData[i].vectorID.y == 1) {
                for (int j = 0; j < m_Entities.Count; j++) {
                    if (m_Entities[j].GridPosition == controlData[i].gridPosition) {
                        if (m_Entities[j].GetComponent<SwitchSpike>() != null) {
                            m_Entities[j].GetComponent<SwitchSpike>().InitOff();
                        }
                    }
                }
            }
            
            // Arrows.
            if (controlData[i].vectorID.y == 2) {
                for (int j = 0; j < m_Entities.Count; j++) {
                    if (m_Entities[j].GridPosition == controlData[i].gridPosition) {
                        if (m_Entities[j].GetComponent<Platform>() != null) {
                            m_Entities[j].GetComponent<Platform>().Init(i, controlData);
                        }
                        if (m_Entities[j].GetComponent<Enemy>() != null) {
                            m_Entities[j].GetComponent<Enemy>().InitPath(i, controlData);
                        }
                    }
                }
            }

            // Offsets.
            if (controlData[i].vectorID.y == 3) {
                for (int j = 0; j < m_Entities.Count; j++) {
                    if (m_Entities[j].GridPosition == controlData[i].gridPosition) {
                        if (m_Entities[j].GetComponent<TimedSpike>() != null) {
                            print("Spike");
                            m_Entities[j].GetComponent<TimedSpike>().Init(controlData[i].vectorID.x);
                        }
                    }
                }
            }

            // Checkpoint.
            if (controlData[i].vectorID == Vector2.zero) {
                Entity newEntity = Instantiate(environment.Checkpoint.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Entity>();
                Vector3 entityPosition = GridToWorldPosition(controlData[i].gridPosition);
                newEntity.Init(controlData[i].gridPosition, entityPosition);
                m_Entities.Add(newEntity);
            }

        }

        List<ElevatorPlatform> elevators = new List<ElevatorPlatform>();
        List<SwitchSpike> switchSpikes = new List<SwitchSpike>();
        List<Lever> levers = new List<Lever>();
        for (int i = 0; i < m_Entities.Count; i++) {
            if (m_Entities[i].GetComponent<ElevatorPlatform>() != null) { elevators.Add(m_Entities[i].GetComponent<ElevatorPlatform>()); }
            if (m_Entities[i].GetComponent<SwitchSpike>() != null) { switchSpikes.Add(m_Entities[i].GetComponent<SwitchSpike>()); }
            if (m_Entities[i].GetComponent<Lever>() != null) { levers.Add(m_Entities[i].GetComponent<Lever>()); }
        }

        for (int i = 0; i < levers.Count; i++) {
            levers[i].Init(elevators, switchSpikes);
        }
    }
    
    #endregion

    /* --- Generics --- */
    #region Generics
    
    public Vector3 GridToWorldPosition(Vector2Int gridPosition) {
        return new Vector3((gridPosition.x + GridOrigin.x) + 0.5f, - (gridPosition.y + GridOrigin.y) + 0.5f, 0f);
    }

    public Vector3Int GridToTilePosition(Vector2Int gridPosition) {
        return new Vector3Int(gridPosition.x + GridOrigin.x, -(gridPosition.y + GridOrigin.y), 0);
    }
    
    #endregion

}

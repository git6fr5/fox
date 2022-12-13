// TODO: Clean

/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using LDtkUnity;
// Platformer.
using Platformer.Character;
using Platformer.CustomTiles;
using Platformer.LevelLoader;

/* --- Definitions --- */
using Game = Platformer.GameManager;

namespace Platformer.LevelLoader {

    /// <summary>
    ///
    /// <summary>
    public class Level : MonoBehaviour {

        public enum State {
            Loaded,
            Unloaded
        }

        #region Fields

        /* --- Constants --- */

        private const float BOUNDARYBOX_SHAVE = 0.775f;
        private static Vector2Int LoadPointID = new Vector2Int(0, 0);

        /* --- Components --- */

        // The trigger box for the level.
        [HideInInspector] 
        private BoxCollider2D m_Box;

        /* --- Members --- */

        [SerializeField, ReadOnly]
        private State m_State = State.Unloaded;  

        [SerializeField, ReadOnly]
        private int m_ID = 0;
        public int ID => m_ID;

        // 
        [SerializeField, ReadOnly] 
        private string m_LevelName = "";
        public string LevelName => m_LevelName;

        //
        [SerializeField, ReadOnly] 
        private LDtkUnity.Level m_LDtkLevel;
        public LDtkUnity.Level LDtkLevel => m_LDtkLevel;

        // 
        [SerializeField, ReadOnly] 
        private Vector2Int m_Dimensions;
        public int Height => m_Dimensions.y;
        public int Width => m_Dimensions.x;

        //
        [SerializeField, ReadOnly] 
        private Vector2Int m_WorldPosition;
        public Vector2Int WorldPosition => m_WorldPosition;
        public int WorldHeight => m_WorldPosition.y;
        public int WorldWidth => m_WorldPosition.x;
        public Vector2 WorldCenter => GetCenter(Width, Height, m_WorldPosition);

        [SerializeField, ReadOnly] 
        private List<Entity> m_Entities = new List<Entity>();

        [SerializeField, ReadOnly] 
        private List<Vector2Int> m_LoadPositions = new List<Vector2Int>();
        public List<Vector2Int> LoadPositions => m_LoadPositions;

        #endregion

        public void PreLoad(int jsonID, LdtkJson  json) {
            transform.localPosition = Vector3.zero;
            ReadJSONData(json, jsonID);
            CreateBoundaryBox();

            List<LDtkTileData> controlData = LDtkReader.GetLayerData(json.Levels[jsonID], LDtkLayer.Control);
            GetLoadPoints(controlData);
        }

        void FixedUpdate() {
            if (m_State != State.Loaded) { return; }

            for (int i = 0; i < m_Dimensions.y; i++) {
                for (int j = 0; j < m_Dimensions.x; j++) {
                    Vector3Int position = new Vector3Int(m_WorldPosition.x + j, m_WorldPosition.y - i, 0);
                    Game.Tilemaps.WaterMap.RefreshTile(position);
                }
            }

        }

        public void ReadJSONData(LdtkJson  json, int jsonID) {
            m_ID = jsonID;
            m_LDtkLevel = json.Levels[jsonID];
            m_LevelName = json.Levels[jsonID].Identifier;
            m_Dimensions.y = (int)(json.Levels[jsonID].PxHei / json.DefaultGridSize);
            m_Dimensions.x = (int)(json.Levels[jsonID].PxWid / json.DefaultGridSize);
            m_WorldPosition.y = (int)(json.Levels[jsonID].WorldY / json.DefaultGridSize);
            m_WorldPosition.x = (int)(json.Levels[jsonID].WorldX / json.DefaultGridSize);
        }

        public void CreateBoundaryBox() {
            m_Box = gameObject.AddComponent<BoxCollider2D>();
            m_Box.isTrigger = true;
            m_Box.size = new Vector2((float)(Width - BOUNDARYBOX_SHAVE), (float)(Height - BOUNDARYBOX_SHAVE));
            m_Box.offset = WorldCenter;
        }

        public void GenerateEntities(List<LDtkTileData> entityData, List<LDtkTileData> controlData, List<Entity> entityReferences) {
            m_Entities.RemoveAll(entity => entity == null);
            Entity.Generate(ref m_Entities, entityData, entityReferences, transform, m_WorldPosition);
            Entity.SetControls(ref m_Entities, controlData);
        }

        public void DestroyEntities() {
            Entity.Destroy(ref m_Entities);
            m_Entities.RemoveAll(entity => entity == null);
        }

        public void GenerateMap(List<LDtkTileData> tileData, GroundTile groundTile, GroundTile maskTile, WaterTile waterTile) {
            List<LDtkTileData> groundData = tileData.FindAll(data => data.VectorID == new Vector2Int(0, 0));
            for (int i = 0; i < tileData.Count; i++) {
                Vector3Int tilePosition = Level.GridToTilePosition(tileData[i].GridPosition, m_WorldPosition);
                Game.Tilemaps.GroundMap.SetTile(tilePosition, groundTile);
                Game.Tilemaps.GroundMaskMap.SetTile(tilePosition, maskTile);
            }            
            List<LDtkTileData> waterData = tileData.FindAll(data => data.VectorID == new Vector2Int(1, 0));
            for (int i = 0; i < tileData.Count; i++) {
                Vector3Int tilePosition = Level.GridToTilePosition(tileData[i].GridPosition, m_WorldPosition);
                Game.Tilemaps.WaterMap.SetTile(tilePosition, waterTile);
            } 
        }

        public void GetLoadPoints(List<LDtkTileData> controlData) {
            m_LoadPositions = new List<Vector2Int>();
            for (int j = 0; j < controlData.Count; j++) {
                if (controlData[j].VectorID == LoadPointID) {
                    m_LoadPositions.Add(controlData[j].GridPosition);
                }
            }
        }

        public void Settings(List<LDtkTileData> controlData) {
            LDtkTileData lightingData = controlData.Find(data => data.VectorID.y == 3);
            LDtkTileData weatherData = controlData.Find(data => data.VectorID.y == 4);
        }

        void OnTriggerEnter2D(Collider2D collider) {

        }

        void OnTriggerExit2D(Collider2D collider) {
            
        }

        public static Vector2 GetCenter(int width, int height, Vector2Int gridOrigin) {
            Vector2Int origin = new Vector2Int(width / 2, height / 2);
            Vector2 offset = new Vector2( width % 2 == 0 ? 0.5f : 0f, height % 2 == 1 ? 0f : -0.5f);
            return (Vector2)GridToWorldPosition(origin, gridOrigin) - offset;
        }
        
        public static Vector3 GridToWorldPosition(Vector2Int gridPosition, Vector2Int gridOrigin) {
            return new Vector3((gridPosition.x + gridOrigin.x) + 0.5f, - (gridPosition.y + gridOrigin.y) + 0.5f, 0f);
        }

        public static Vector3Int GridToTilePosition(Vector2Int gridPosition, Vector2Int gridOrigin) {
            return new Vector3Int(gridPosition.x + gridOrigin.x, -(gridPosition.y + gridOrigin.y), 0);
        }

    }

}

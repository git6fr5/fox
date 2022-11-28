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
using Platformer.Utilites;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.LevelLoader {

    /// <summary>
    ///
    /// <summary>
    public class Level : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // References to all the maps in the game.
        public static Tilemap GroundMap;
        public static Tilemap GroundMapMask;
        public static Tilemap WaterMap;

        // Tracks the load state of this level.
        [SerializeField, ReadOnly] private bool m_Loaded;
        [SerializeField, ReadOnly] private bool m_Unloading;
        [SerializeField, ReadOnly] private float m_UnloadTicks;
        [SerializeField, ReadOnly] private List<Entity> m_Entities = new List<Entity>();

        // The trigger box for the level.
        [HideInInspector] private BoxCollider2D m_Box;

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

        // Position.
        public Vector2Int GridOrigin => new Vector2Int(m_WorldWidth, m_WorldHeight);
        public Vector2 WorldCenter => GetCenter(m_Width, m_Height, GridOrigin);

        // Controls.
        private static Vector2Int LoadPointID = new Vector2Int(0, 0);
        [HideInInspector] private List<Vector2Int> m_LoadPositions = new List<Vector2Int>();

        #endregion

        /* --- Initialization --- */
        #region Initialization

        // public void Reset() {
        //     LDtkLoader.UnloadEntities(this);
        //     LDtkLoader.UnloadEntities(this);
        // }

        public void Init(int jsonID, LdtkJson  json) {
            transform.localPosition = Vector3.zero;
            ReadJSONData(json, jsonID);
            InitializeBoundaryBox();
        }

        void FixedUpdate() {
            if (!m_Loaded) {
                return;
            }

            bool finished = Timer.TickDownIf(ref m_UnloadTicks, Time.fixedDeltaTime, m_Unloading);
            if (finished) {
                LDtkLoader.UnloadEntities(this);
                m_Loaded = false;
            }

            for (int i = 0; i < m_Height; i++) {
                for (int j = 0; j < m_Width; j++) {
                    Vector3Int position = new Vector3Int(GridOrigin.x + j, GridOrigin.y - i, 0);
                    Level.WaterMap.RefreshTile(position);
                }
            }

        }

        public void ReadJSONData(LdtkJson  json, int jsonID) {
            m_ID = jsonID;
            m_LDtkLevel = json.Levels[jsonID];
            m_LevelName = json.Levels[jsonID].Identifier;
            m_Height = (int)(json.Levels[jsonID].PxHei / json.DefaultGridSize);
            m_Width = (int)(json.Levels[jsonID].PxWid / json.DefaultGridSize);
            m_WorldHeight = (int)(json.Levels[jsonID].WorldY / json.DefaultGridSize);
            m_WorldWidth = (int)(json.Levels[jsonID].WorldX / json.DefaultGridSize);

            List<LDtkTileData> controlData = LDtkReader.GetLayerData(json.Levels[jsonID], LDtkLayer.Control);
            m_LoadPositions = new List<Vector2Int>();

            for (int j = 0; j < controlData.Count; j++) {
                if (controlData[j].VectorID == LoadPointID) {
                    m_LoadPositions.Add(controlData[j].GridPosition);
                }
            }

        }

        public void InitializeBoundaryBox() {
            gameObject.layer = LayerMask.NameToLayer("UI");
            m_Box = gameObject.AddComponent<BoxCollider2D>();
            m_Box.isTrigger = true;
            float shave = 0.775f;
            m_Box.size = new Vector2((float)(m_Width - shave), (float)(m_Height - shave));
            m_Box.offset = WorldCenter;
        }

        public static void InitializeGroundLayer(Transform transform) {
            GroundMap = new GameObject("Ground", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
            GroundMap.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Foreground;
            GroundMap.color = Screen.ForegroundColorShift;

            GroundMap.gameObject.AddComponent<Rigidbody2D>();
            GroundMap.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            GroundMap.gameObject.AddComponent<CompositeCollider2D>();
            GroundMap.gameObject.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
            GroundMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
            // GroundMap.gameObject.AddComponent<ShadowCaster2DTileMap>();

            GroundMap.transform.SetParent(transform);
            GroundMap.transform.localPosition = Vector3.zero;
            GroundMap.gameObject.layer = LayerMask.NameToLayer("Ground");

            GroundMapMask = new GameObject("Ground Mask", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            GroundMapMask.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Midground;
            GroundMapMask.GetComponent<TilemapRenderer>().sortingOrder = 10000;
            // GroundMapMask.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            GroundMapMask.transform.SetParent(transform);
            GroundMapMask.transform.localPosition = Vector3.zero;

            // Outline.Add(GroundMap.GetComponent<TilemapRenderer>(), 0.5f, 16f);
            // Outline.Set(GroundMap.GetComponent<TilemapRenderer>(), Color.black);

        }

        public static void InitializeWaterLayer(Transform transform) {
            WaterMap = new GameObject("Water", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
            WaterMap.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Foreground;
            WaterMap.GetComponent<TilemapCollider2D>().isTrigger = true;
            WaterMap.transform.SetParent(transform);
            WaterMap.transform.localPosition = Vector3.zero;
            WaterMap.gameObject.layer = LayerMask.NameToLayer("Water");
        }

        #endregion

        /* --- Generation --- */
        #region Generation

        public void MoveToLoadPoint(Transform playerTransform) {
            if (m_LoadPositions != null && m_LoadPositions.Count > 0) {
                Vector3 position = GridToWorldPosition(m_LoadPositions[0], GridOrigin);
                playerTransform.position = position;
                Rigidbody2D body = playerTransform.GetComponent<Rigidbody2D>();
                if (body != null) {
                    body.velocity = Vector2.zero;
                }
            }
        }
        
        public void GenerateEntities(List<LDtkTileData> entityData, List<LDtkTileData> controlData, List<Entity> entityReferences) {
            m_Entities.RemoveAll(entity => entity == null);
            Entity.Generate(ref m_Entities, entityData, entityReferences, transform, GridOrigin);
            Entity.SetControls(ref m_Entities, controlData);
        }

        public void DestroyEntities() {
            Entity.Destroy(ref m_Entities);
            m_Entities.RemoveAll(entity => entity == null);
        }

        public void GenerateMap(List<LDtkTileData> tileData, GroundTile groundTile, GroundTile maskTile, WaterTile waterTile) {
            List<LDtkTileData> groundData = tileData.FindAll(tile => tile.VectorID == new Vector2Int(0, 0));
            List<LDtkTileData> waterData = tileData.FindAll(tile => tile.VectorID == new Vector2Int(1, 0));
            GenerateGround(groundData, groundTile, maskTile);
            GenerateWater(waterData, waterTile);
        }

        public void GenerateGround(List<LDtkTileData> tileData, GroundTile tile, GroundTile maskTile) {
            for (int i = 0; i < tileData.Count; i++) {
                Vector3Int tilePosition = Level.GridToTilePosition(tileData[i].GridPosition, GridOrigin);
                GroundMap.SetTile(tilePosition, tile);
                GroundMapMask.SetTile(tilePosition, maskTile);
            }
        }

        public void GenerateWater(List<LDtkTileData> tileData, WaterTile tile) {
            for (int i = 0; i < tileData.Count; i++) {
                Vector3Int tilePosition = Level.GridToTilePosition(tileData[i].GridPosition, GridOrigin);
                WaterMap.SetTile(tilePosition, tile);
            }
        }

        public void Settings(List<LDtkTileData> controlData) {
            // Lighting
            LDtkTileData lightingData = controlData.Find(data => data.VectorID.y == 3);
            // Screen.SetLighting(lightingData.VectorID.x);
            
            // Weather.
            LDtkTileData weatherData = controlData.Find(data => data.VectorID.y == 4);
            // Screen.SetWeather(weatherData.VectorID.x);
        }

        #endregion

        /* --- Entering --- */
        #region Entering

        // public void Reload() {
        //     LDtkLoader.UnloadEntities(this);
        //     LDtkLoader.LoadEntities(this, m_LDtkLevel, Game.LevelLoader.LevelEnvironment);
        // }
        
        void OnTriggerEnter2D(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            bool player = character != null && character.IsPlayer;

            if (player) {

                character.OverrideFall(false);
                character.OverrideMovement(false);
                character.DisableAllAbilityActions();

                Platformer.Rendering.Screen.Recolor(Screen.DefaultPalette);

                if (m_Loaded) {
                    m_Unloading = false;
                }
                else {
                    LDtkLoader.LoadEntities(this, m_LDtkLevel, Game.LevelLoader.LevelEnvironment);
                    m_Loaded = true;
                }

                LightSwitch(true);
                // TODO: Fix
                // character.CurrentMinimap.Load(this);
                Screen.Instance.Snap(WorldCenter);
                Screen.Instance.Shape(new Vector2Int(m_Width, m_Height));
                
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            bool player = character != null && character.IsPlayer;
            if (player) {
                Timer.Start(ref m_UnloadTicks, 0.1f);
                LightSwitch(false);
                m_Unloading = true;
            }
        }

        public void LightSwitch(bool on) {
            // for (int i = 0; i < m_Entities.Count; i++) {
            //     if (m_Entities[i] != null && m_Entities[i].GetComponent<UnityEngine.Rendering.Universal.Light2D>()) {
            //         m_Entities[i].gameObject.SetActive(on);
            //     }
            // }
        }
        
        #endregion

        /* --- Generics --- */
        #region Generics

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
        
        #endregion

    }

}

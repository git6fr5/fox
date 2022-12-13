/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.LevelLoader;
using Platformer.CustomTiles;

/* --- Definitions --- */
using Game = Platformer.GameManager;

namespace Platformer.LevelLoader {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class LDtkLoader: MonoBehaviour {
        
        /* --- Variables --- */
        #region Variables

        [SerializeField] 
        private string m_OpeningLevel;
        public string OpeningLevel => m_OpeningLevel;
        
        // The given LDtk file.
        [SerializeField] 
        private LDtkComponentProject m_LDtkData;

        [HideInInspector] 
        private LdtkJson m_JSON;

        // A reference to all the created levels.
        [HideInInspector] 
        private List<Level> m_Levels;
        public List<Level> Levels => m_Levels;
        
        #endregion

        // Initializes the world.
        public void OnGameLoad() {
            // Read and collect the data.
            m_JSON = m_LDtkData.FromJson();
            m_Levels = Collect(m_JSON, transform);
            // Load the maps for all the levels.
            LoadMaps(m_Levels);
            MoveToLoadPoint(m_OpeningLevel, Game.MainPlayer.transform);
        }
        
        // Collects all the levels from the LDtk file.
        private static List<Level> Collect(LdtkJson json, Transform transform) {
            List<Level> levels = new List<Level>();
            for (int i = 0; i < json.Levels.Length; i++) {
                Level level = new GameObject(json.Levels[i].Identifier, typeof(Level)).GetComponent<Level>();
                level.transform.SetParent(transform);
                level.PreLoad(i, json);
                levels.Add(level);
            }
            return levels;
        }

        // Loads the map layouts for all the given levels.
        public static void LoadMaps(List<Level> levels) {
            // Load the custom tile mappings.
            CustomTileMappings.CreateGroundTileMapping();
            CustomTileMappings.CreateWaterTileMapping();

            // Itterate through and load all the level data.
            for (int i = 0; i < levels.Count; i++) {
                LDtkUnity.Level ldtkLevel = levels[i].LDtkLevel;
                List<LDtkTileData> tileData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Ground);
                levels[i].GenerateMap(tileData, Game.Tilemaps.Ground, Game.Tilemaps.GroundMask, Game.Tilemaps.Water);
            }

            // Refresh all the maps once after all the data has been loaded.
            Game.Tilemaps.WaterMap.RefreshAllTiles();
            Game.Tilemaps.GroundMap.RefreshAllTiles();
            Game.Tilemaps.GroundMaskMap.RefreshAllTiles();
            // Level.GroundMap.GetComponent<ShadowCaster2DTileMap>().Generate(0.5f);

        }

        public void MoveToLoadPoint(string levelName, Transform playerTransform) {
            Level level = m_Levels.Find(level => level.LevelName == levelName);
            if (level.LoadPositions != null && level.LoadPositions.Count > 0) {
                Vector3 position = Level.GridToWorldPosition(level.LoadPositions[0], level.WorldPosition);
                playerTransform.position = position;
                Rigidbody2D body = playerTransform.GetComponent<Rigidbody2D>();
                if (body != null) {
                    body.velocity = Vector2.zero;
                }
            }
        }

        // Loads the entities for 
        public static void LoadEntities(Level level, LDtkUnity.Level ldtkLevel) {
            if (ldtkLevel != null) {
                // Load the data.
                List<LDtkTileData> entityData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Entity);
                List<LDtkTileData> controlData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Control);

                // Load the level.
                level.GenerateEntities(entityData, controlData, Game.Entities.All);
                level.Settings(controlData);                
            }
        }

        public static void UnloadEntities(Level level) {
            level.DestroyEntities();
        }

    }



}
    
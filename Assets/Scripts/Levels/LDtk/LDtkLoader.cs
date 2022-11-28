/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.CustomTiles;
using Platformer.LevelLoader;
using Platformer.Rendering;

namespace Platformer.LevelLoader {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class LDtkLoader: MonoBehaviour {
        
        /* --- Variables --- */
        #region Variables
        
        // The given LDtk file.
        [SerializeField] private LDtkComponentProject m_LDtkData;
        [HideInInspector] private LdtkJson m_JSON;

        // The environment this scene takes place in.
        [SerializeField] private Environment m_Environment;
        public Environment LevelEnvironment => m_Environment;

        // A reference to all the created levels.
        [HideInInspector] private List<Level> m_Levels;
        public List<Level> Levels => m_Levels;
        
        #endregion

        // Initializes the world.
        public void Init() {
            // Read and collect the data.
            m_Environment.Init();
            m_JSON = m_LDtkData.FromJson();
            m_Levels = Collect(m_JSON, transform);
            // Load the maps for all the levels.
            LoadMaps(m_Levels, m_Environment);
        }
        
        // Collects all the levels from the LDtk file.
        private static List<Level> Collect(LdtkJson json, Transform transform) {
            List<Level> levels = new List<Level>();
            for (int i = 0; i < json.Levels.Length; i++) {
                Level level = new GameObject(json.Levels[i].Identifier, typeof(Level)).GetComponent<Level>();
                level.transform.SetParent(transform);
                level.Init(i, json);
                levels.Add(level);
            }
            return levels;
        }

        // Loads the map layouts for all the given levels.
        public static void LoadMaps(List<Level> levels, Environment environment) {
            // Load the custom tile mappings.
            CustomTileMappings.CreateGroundTileMapping();
            CustomTileMappings.CreateWaterTileMapping();

            // Itterate through and load all the level data.
            for (int i = 0; i < levels.Count; i++) {
                LDtkUnity.Level ldtkLevel = levels[i].LDtkLevel;
                List<LDtkTileData> tileData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Ground);
                levels[i].GenerateMap(tileData, environment.Ground, environment.GroundMask, environment.Water);
            }

            // Refresh all the maps once after all the data has been loaded.
            Level.WaterMap.RefreshAllTiles();
            Level.GroundMap.RefreshAllTiles();
            Level.GroundMapMask.RefreshAllTiles();
            // Level.GroundMap.GetComponent<ShadowCaster2DTileMap>().Generate(0.5f);

        }

        // Sets the loadpoint based on the given level name.
        public void SetLoadPoint(string levelName, Transform playerTransform = null) {
            Level level = m_Levels.Find(level => level.LevelName == levelName);
            if (playerTransform != null) {
                level.MoveToLoadPoint(playerTransform);
                playerTransform.gameObject.SetActive(true);
            }
        }

        // Loads the entities for 
        public static void LoadEntities(Level level, LDtkUnity.Level ldtkLevel, Environment environment) {
            if (ldtkLevel != null) {
                // Load the data.
                List<LDtkTileData> entityData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Entity);
                List<LDtkTileData> controlData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Control);

                // Load the level.
                level.GenerateEntities(entityData, controlData, environment.Entities);
                level.Settings(controlData);                
            }
        }

        public static void UnloadEntities(Level level) {
            level.DestroyEntities();
        }

    }



}
    
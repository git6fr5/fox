/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
using Monet;

namespace Monet {

    /// <summary>
    /// 
    /// </summary>
    public class LDtkLoader: MonoBehaviour {
        
        // Components.
        [SerializeField] private LDtkComponentProject m_LDtkData;
        [HideInInspector] private LdtkJson m_JSON;

        [HideInInspector] private List<Level> m_Levels;
        public List<Level> Levels => m_Levels;

        public void Init() {
            m_JSON = m_LDtkData.FromJson();
            m_Levels = Collect(m_JSON, transform);
            LoadAllTiles(m_Levels);
        }
        
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

        
        public static void LoadAllTiles(List<Level> levels) {
            GroundTileEditor.CreateMapping();
            for (int i = 0; i < levels.Count; i++) {
                LDtkUnity.Level ldtkLevel = levels[i].LDtkLevel;
                List<LDtkTileData> waterData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Water);
                List<LDtkTileData> groundData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Ground);
                levels[i].GenerateGround(groundData, Game.Environment.Ground);
                levels[i].GenerateWater(waterData, Game.Environment.Water);

            }
            Level.WaterMap.RefreshAllTiles();
            Level.GroundMap.RefreshAllTiles();
        }

        public void Open(string levelName, Transform playerTransform = null) {
            Level level = m_Levels.Find(level => level.LevelName == levelName);
            if (level != null) {
                Open(level);
            }
            if (playerTransform != null) {
                level.MoveToLoadPoint(playerTransform);
            }
        }

        public static void Open(Level level) {
            LDtkUnity.Level ldtkLevel = level.LDtkLevel;
            Load(level, ldtkLevel);
        }

        public static void Load(Level level, LDtkUnity.Level ldtkLevel) {
            if (ldtkLevel != null) {
                // Load the data.
                List<LDtkTileData> entityData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Entity);
                List<LDtkTileData> controlData = LDtkReader.GetLayerData(ldtkLevel, LDtkLayer.Control);

                // Load the level.
                level.GenerateEntities(entityData, controlData, Game.Environment.Entities);
                
            }
        }

        public static void Close(Level level) {
            level.DestroyEntities();
        }

    }



}
    
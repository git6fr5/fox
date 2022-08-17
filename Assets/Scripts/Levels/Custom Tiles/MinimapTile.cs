// TODO: Clean

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

using Platformer.Rendering;
using Platformer.CustomTiles;
using Platformer.LevelLoader;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platformer.CustomTiles {

    [Serializable]
    public class EntityMarker {
        [SerializeField] private Sprite m_Sprite;
        public Sprite Sprite => m_Sprite;
        [SerializeField] private Entity m_Entity;
        public Entity MarkedEntity => m_Entity;
    }

    [Serializable]
    public class MinimapTile : Tile {

        public enum Type {
            Ground,
            Water,
            Entity
        }

        [SerializeField] private Sprite m_GroundMarker;
        [SerializeField] private Sprite m_WaterMarker;
        [SerializeField] private List<EntityMarker> m_EntityMarkers;
        
        private static Dictionary<Vector3Int, Type> m_TypeDict = new Dictionary<Vector3Int, Type>();
        private static Dictionary<Vector3Int, Vector2Int> m_VectorIdDict = new Dictionary<Vector3Int, Vector2Int>();

        public static void SetVariation(Vector3Int position, MinimapTile.Type type) {
            if (!m_TypeDict.ContainsKey(position)) {
                m_TypeDict.Add(position, type);
            }
            else {
                m_TypeDict[position] = type;
            }
        }

        public static void SetVariation(Vector3Int position, MinimapTile.Type type, Vector2Int vector2Int) {
            if (!m_TypeDict.ContainsKey(position)) {
                m_TypeDict.Add(position, type);
            }
            else {
                m_TypeDict[position] = type;

                if (type == Type.Entity) {
                    if (!m_VectorIdDict.ContainsKey(position)) {
                        m_VectorIdDict.Add(position, vector2Int);
                    }
                    else {
                        m_VectorIdDict[position] = vector2Int;
                    }
                }

            }
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            if (m_TypeDict.ContainsKey(position)) {
                if (m_TypeDict[position] == MinimapTile.Type.Ground) {
                    tileData.sprite = m_GroundMarker;
                }
                else if (m_TypeDict[position] == MinimapTile.Type.Water) {
                    tileData.sprite = m_WaterMarker;
                }
                else if (m_TypeDict[position] == MinimapTile.Type.Entity) {
                    if (m_VectorIdDict.ContainsKey(position)) {
                        Vector2Int vectorID = m_VectorIdDict[position];
                        EntityMarker marker = m_EntityMarkers.Find(marker => marker.MarkedEntity.VectorID == vectorID);
                        if (marker != null) {
                            tileData.sprite = marker.Sprite;
                        }
                    } 
                }
            }
            tileData.color = new Color(1f, 1f, 1f, 1f);
            tileData.colliderType = Tile.ColliderType.None;
        }

        #if UNITY_EDITOR
        [MenuItem("Assets/Create/2D/Custom Tiles/MinimapTile")]
        public static void CreateMinimapTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save MinimapTile", "New MinimapTile", "Asset", "Save MinimapTile", "Assets");
            if (path == "") { return; }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MinimapTile>(), path);
        }
        #endif

    }

}


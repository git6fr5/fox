/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LDtkUnity;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class Minimap : MonoBehaviour {

        [SerializeField] private Tilemap m_Map;
        [SerializeField] private MinimapTile m_MinimapTile;

        [SerializeField] private Sprite m_PlayerMarker;
        [HideInInspector] private Vector3 m_PlayerPosition;
        
        void Update() {
            m_PlayerPosition = Game.MainPlayer.transform.position;
            // SetPlayerMarker();
        }

        public void Load(Level level) {
            List<LDtkTileData> groundData = LDtkReader.GetLayerData(level.LDtkLevel, LDtkLayer.Ground);
            List<LDtkTileData> waterData = LDtkReader.GetLayerData(level.LDtkLevel, LDtkLayer.Water);
            List<LDtkTileData> entityData = LDtkReader.GetLayerData(level.LDtkLevel, LDtkLayer.Entity);
            
            AddTiles(m_Map, groundData, m_MinimapTile, MinimapTile.Type.Ground, level.GridOrigin);
            AddTiles(m_Map, waterData, m_MinimapTile, MinimapTile.Type.Water, level.GridOrigin);

        }

        public static void AddTiles(Tilemap minimap, List<LDtkTileData> data, TileBase tile, MinimapTile.Type type, Vector2Int origin) {
            for (int i = 0; i < data.Count; i++) {
                Vector3Int position = Level.GridToTilePosition(data[i].GridPosition, origin);
                MinimapTile.SetVariation(position, type);
                minimap.SetTile(position, tile);
            }
        }

        public void AddTargets(List<LDtkTileData> entityData, Level level) {
            
            // for (int i = 0; i < entityData.Count; i++) {
            //     Entity entityBase = Environment.GetEntityByVectorID(entityData[i].vectorID, worldLoader.Environment.Entities);
            //     if (entityBase != null) {

            //         Vector3Int position = level.GridToTilePosition(entityData[i].gridPosition);

            //         if (entityBase.gameObject.GetComponent<Platform>()) {
            //             m_Map.SetTile(position, platformTile);
            //         }

            //         if (entityBase.gameObject.tag == "Respawn Anchor") {
            //             m_Map.SetTile(position, respawnTile);
            //         }

            //         if (entityBase.gameObject.tag.Contains("scroll")) {
            //             m_Map.SetTile(position, scrollTile);
            //         }

            //         if (entityBase.gameObject.tag == "Shopkeeper") {
            //             m_Map.SetTile(position, shopKeeperTile);
            //         }

            //         if (entityBase.gameObject.tag == "Goldbox") {
            //             m_Map.SetTile(position, goldTile);
            //         }
                    
            //     }
                
            // }

        }

    }
}
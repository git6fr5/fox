using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using Platformer.LevelLoader;
using Platformer.CustomTiles;

namespace Platformer.LevelLoader {

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    public class Environment : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // Characters.
        [HideInInspector] private List<Entity> m_Entities;
        public List<Entity> Entities => m_Entities;

        // Tiles.
        [SerializeField] private GroundTile m_Ground;
        public GroundTile Ground => m_Ground; 
        [SerializeField] private GroundTile m_GroundMask;
        public GroundTile GroundMask => m_GroundMask; 
        [SerializeField] private WaterTile m_Water;
        public WaterTile Water => m_Water;
        
        #endregion

        /* --- Initialization --- */
        #region Initialization
        
        public void Init() {
            FindEntities(transform, ref m_Entities);
        }

        private static  void FindEntities(Transform parent, ref List<Entity> entityList) {
            entityList = new List<Entity>();
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child, ref entityList);
            }
        }

        private static void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {
            // If we've found an entity, don't go any deeper.
            if (parent.GetComponent<Entity>() != null) {
                entityList.Add(parent.GetComponent<Entity>());
            }
            else if (parent.childCount > 0) {
                foreach (Transform child in parent) {
                    FindAllEntitiesInTransform(child, ref entityList);
                }
            }
        }
        
        public static Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
            for (int i = 0; i < entityList.Count; i++) {
                if (entityList[i].VectorID == vectorID) {
                    return entityList[i];
                }
            }
            return null;
        }
        
        #endregion

    }
    
}
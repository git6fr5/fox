/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.LevelLoader;

namespace Platformer.LevelLoader {

    ///<summary>
    ///
    ///<summary>
    public class EntityManager : MonoBehaviour {

        
        // Characters.
        [HideInInspector] 
        private List<Entity> m_AllEntities;
        public List<Entity> All => m_AllEntities;

        public void OnGameLoad() {
            FindEntities(transform, ref m_AllEntities);
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


    }
}
/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.LevelLoader;

namespace Platformer.LevelLoader {

    /// <summary>
    /// An entity object readable by the level loader.
    /// <summary>
    public class Entity : MonoBehaviour {

        #region Variables

        // Whether this entity should be duplicated or taken directly.       
        [SerializeField] bool m_Singular = false;
        public bool Singular => m_Singular;

        // Whether this entity can be unloaded.
        [SerializeField] bool m_Unloadable = true;
        public bool Unloadable => m_Unloadable;

        // The mapping from LDtk to this entity.
        [SerializeField] private Vector2Int m_VectorID;
        public Vector2Int VectorID => m_VectorID;

        // The grid space coordinates this entity should be loaded at.
        [SerializeField, ReadOnly] private Vector2Int m_GridPosition;
        public Vector2Int GridPosition => m_GridPosition;

        // The world space offset this entity should be loaded at.
        [SerializeField] private Vector2 m_LoadOffset;
        public Vector2 LoadOffset => m_LoadOffset;
        
        #endregion
        
        // Initialize this entity with the given settings.
        public void Init(Vector2Int gridPosition, Vector3 position) {
            m_GridPosition = gridPosition;
            transform.localPosition = position + (Vector3)m_LoadOffset;
            gameObject.SetActive(true);
        }

                // Destroy and clean through a list of entities.
        public static void Destroy(ref List<Entity> entities) {
            if (entities == null || entities.Count == 0) { return; }
            
            for (int i = 0; i < entities.Count; i++) {
                bool destroy = entities[i] != null && entities[i].Unloadable;
                if (destroy) {
                    Destroy(entities[i].gameObject);
                }
            }
        }

        // Duplicate an entity.
        private Entity Duplicate(Transform parent) {
            Entity entity = Instantiate(gameObject, Vector3.zero, transform.localRotation, parent).GetComponent<Entity>();
            if (Singular) {
                Destroy(gameObject);
            }
            return entity;
        }

        // How to interpret the control data for this specific entity.
        public virtual void OnControl(int index, List<LDtkTileData> controlData) { }

        // Generate a entities from a list.
        public static void Generate(ref List<Entity> entities, List<LDtkTileData> entityData,  List<Entity> entityReferences, Transform parent, Vector2Int origin) {

            for (int i = 0; i < entityData.Count; i++) {
                bool preloaded = entities.Find(ent => ent.VectorID == entityData[i].VectorID && ent.GridPosition == entityData[i].GridPosition) != null;
                Entity entity = Environment.GetEntityByVectorID(entityData[i].VectorID, entityReferences);
                if (entity != null && !preloaded) {
                    entity = entity.Duplicate(parent);
                    entities.Add(entity);
                    Vector3 entityPosition = Level.GridToWorldPosition(entityData[i].GridPosition, origin);
                    entity.Init(entityData[i].GridPosition, entityPosition);
                }
            }

        }

        // Set the control data for the entites.
        public static void SetControls(ref List<Entity> entities, List<LDtkTileData> controlData) {
            for (int i = 0; i < controlData.Count; i++) {
                Entity entity = entities.Find(entity => entity.GridPosition == controlData[i].GridPosition);
                if (entity != null) {
                    entity.OnControl(i, controlData);
                }
            }
        }

    }

}

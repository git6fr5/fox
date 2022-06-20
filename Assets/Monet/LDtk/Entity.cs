using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    /// <summary>
    /// An entity object readable by the level loader.
    /// <summary>
    public class Entity : MonoBehaviour {

        [SerializeField] bool m_Singular;
        public bool Singular => m_Singular;
        [SerializeField] bool m_Unloadable;
        public bool Unloadable => m_Unloadable;
        [SerializeField] private Vector2Int m_VectorID;
        public Vector2Int VectorID => m_VectorID;

        [SerializeField, ReadOnly] private Vector2Int m_GridPosition;
        public Vector2Int GridPosition => m_GridPosition;

        public void Init(Vector2Int gridPosition, Vector3 position) {
            m_GridPosition = gridPosition;
            transform.localPosition = position;
            gameObject.SetActive(true);
        }

        public virtual void OnControl(int index, List<LDtkTileData> controlData) {

        }

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

        public static void SetControls(ref List<Entity> entities, List<LDtkTileData> controlData) {
            for (int i = 0; i < controlData.Count; i++) {
                Entity entity = entities.Find(entity => entity.GridPosition == controlData[i].GridPosition);
                if (entity != null) {
                    entity.OnControl(i, controlData);
                }
            }
        }

        public static void Destroy(ref List<Entity> entities) {
            if (entities == null || entities.Count == 0) { return; }
            
            for (int i = 0; i < entities.Count; i++) {
                bool destroy = entities[i] != null && entities[i].Unloadable;
                if (destroy) {
                    Destroy(entities[i].gameObject);
                }
            }
        }

        private Entity Duplicate(Transform parent) {
            Entity entity = Instantiate(gameObject, Vector3.zero, Quaternion.identity, parent).GetComponent<Entity>();
            if (Singular) {
                Destroy(gameObject);
            }
            return entity;
        }

    }

}

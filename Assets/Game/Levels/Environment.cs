using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Stores specific data on how to generate the level.
/// </summary>
public class Environment : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Characters.
    [SerializeField] private Transform m_CharacterTransform;
    [HideInInspector] private List<Entity> m_Characters;
    public List<Entity> Characters => m_Characters;

    // Tile.
    [SerializeField] private RuleTile m_Tile;
    public RuleTile Tile => m_Tile; 

    #endregion

    /* --- Initialization --- */
    #region Initialization
    
    public void Init() {
        FindEntities(m_CharacterTransform, ref m_Characters);
    }

    private void FindEntities(Transform parent, ref List<Entity> entityList) {
        entityList = new List<Entity>();
        foreach (Transform child in parent) {
            FindAllEntitiesInTransform(child, ref entityList);
        }
    }

    private void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {
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
    
    #endregion

    /* --- Generics --- */
    #region Generics
    
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

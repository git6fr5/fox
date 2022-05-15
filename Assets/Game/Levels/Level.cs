/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/* --- Definitions --- */
using LDtkTileData = LevelLoader.LDtkTileData;

/// <summary>
///
/// <summary>
public class Level : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    [SerializeField] private string m_LevelName;
    public string LevelName => m_LevelName;
    
    [SerializeField] private Tilemap m_Map;
    [SerializeField] private List<Entity> m_Entities;
    
    #endregion

    /* --- Generation --- */
    #region Generation
    
    public void GenerateEntities(List<LDtkTileData> entityData, List<Entity> entityList) {
        for (int i = 0; i < entityData.Count; i++) {
            Entity entityBase = Environment.GetEntityByVectorID(entityData[i].vectorID, entityList);
            if (entityBase != null) {
                Entity newEntity = Instantiate(entityBase.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Entity>();
                Vector3 entityPosition = GridToWorldPosition(entityData[i].gridPosition);
                newEntity.Init(entityPosition);
                m_Entities.Add(newEntity);
            }
        }

    }

    public void GenerateTiles(List<LDtkTileData> tileData, RuleTile tile) {
        for (int i = 0; i < tileData.Count; i++) {
            Vector3Int tilePosition = GridToTilePosition(tileData[i].gridPosition);
            m_Map.SetTile(tilePosition, tile);
        }

    }
    
    #endregion

    /* --- Generics --- */
    #region Generics
    
    private Vector3 GridToWorldPosition(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + 0.5f, - gridPosition.y + 0.5f, 0f);
    }

    private Vector3Int GridToTilePosition(Vector2Int gridPosition) {
        return new Vector3Int(gridPosition.x, -gridPosition.y, 0);
    }
    
    #endregion

}

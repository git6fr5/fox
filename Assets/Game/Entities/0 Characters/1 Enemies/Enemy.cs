/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using LDtkTileData = LevelLoader.LDtkTileData;

///<summary>
///
///<summary>
public class Enemy : Input {

    /* --- Variables --- */
    #region Variables
    
    [HideInInspector] protected Vector3[] m_Path = null;
    [SerializeField, ReadOnly] protected int m_PathIndex;

    [SerializeField] private float m_PathDuration;
    [SerializeField] private float m_Ticks;

    #endregion

    public void InitPath(int index, List<LDtkTileData> controlData) {

        Vector2 _direction = Quaternion.Euler(0f, 0f, 90f * controlData[index].vectorID.x) * Vector2.right;
        Vector2Int direction = new Vector2Int((int)Mathf.Round(_direction.x), (int)Mathf.Round(_direction.y));
        print(direction);
        direction.y *= -1;

        List<LDtkTileData> nodes = new List<LDtkTileData>();
         for (int i = 0; i < controlData.Count; i++) {
            if (controlData[i].vectorID == new Vector2Int(0, 1)) {
                nodes.Add(controlData[i]);
            }
        }

        int distance = 0;
        bool continueSearch = true;
        while (continueSearch && distance < 50) {
            distance += 1;
            
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].gridPosition == controlData[index].gridPosition + distance * direction) {
                    continueSearch = false;
                    break;
                }
            }

        }

        List<Vector3> l_Path = new List<Vector3>();
        l_Path.Add(transform.position);
        l_Path.Add(transform.position + (distance) * (Vector3)_direction);
        m_Path = l_Path.ToArray();

    }

}
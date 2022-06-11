using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    [SerializeField] private Vector2Int m_VectorID;
    public Vector2Int VectorID => m_VectorID;

    [SerializeField, ReadOnly] private Vector2Int m_GridPosition;
    public Vector2Int GridPosition => m_GridPosition;

    public void Init(Vector2Int gridPosition, Vector3 position) {
        m_GridPosition = gridPosition;
        transform.localPosition = position;
        gameObject.SetActive(true);
    }

}

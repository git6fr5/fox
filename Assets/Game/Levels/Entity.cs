using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    [SerializeField] private Vector2Int m_VectorID;
    public Vector2Int VectorID => m_VectorID;

    public void Init(Vector3 position) {
        transform.localPosition = position;
        gameObject.SetActive(true);
    }

}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Enemy : Input {

        [HideInInspector] protected Vector3 m_Origin;
        [SerializeField] protected Vector3[] m_Path = null;
        [SerializeField, ReadOnly] protected int m_PathIndex;

        public void Init(Vector3[] path) {
            m_Origin = transform.position;
            m_Path = path;
        }

    }
}
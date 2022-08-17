/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundLayer : MonoBehaviour {

        // [SerializeField] private Transform m_HeightTransform;
        [SerializeField] private Vector2 m_Node;
        public float Height => m_Node.y;
        public float Width => Mathf.Abs(m_Node.x);
        [SerializeField] private float m_Depth;
        public float Depth => m_Depth;
        [SerializeField] private bool m_Perspective = true;
        public bool Perspective => m_Perspective;
        [SerializeField] private bool m_Shadow = false;
        public bool Shadow => m_Shadow;

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        public Vector3 Origin => m_Origin;

        void Start() {
            m_Origin = transform.localPosition;
        }

        void Update() {

            if (!Application.isPlaying) {

                GetComponent<SpriteRenderer>().sortingOrder = (int)Mathf.Round(m_Depth);

            }

        }

        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(m_Node, 0.5f);
        }
        
    }
}
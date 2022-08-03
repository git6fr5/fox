/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// Leaves a trail behind it.
    ///<summary>
    [RequireComponent(typeof(LineRenderer))]
    public class Trail : MonoBehaviour {

        #region Variables

        // Components.
        [HideInInspector] private LineRenderer m_LineRenderer;

        // Trail.
        [SerializeField] private float m_Width;
        [SerializeField] private float m_FadeInterval;
        [SerializeField] private float m_TrailPrecision;
        [SerializeField, ReadOnly] public List<Vector3> m_Trail;
        [SerializeField, ReadOnly] private Vector3 m_CachedPosition;

        #endregion

        // Runs once before the first frame.
        void Start() {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.endWidth = 0f;
            m_LineRenderer.startWidth = m_Width;
            m_Trail = new List<Vector3>();
        }

        // Runs once every frame.
        void Update() {
            float dx = (m_CachedPosition - transform.position).magnitude;
            if (dx > m_TrailPrecision) {
                Add();
            }
        }

        // Adds a point
        void Add() {
            m_Trail.Insert(0, transform.position);
            m_CachedPosition = transform.position;
            m_LineRenderer.positionCount = m_Trail.Count;
            m_LineRenderer.SetPositions(m_Trail.ToArray());
            StartCoroutine(IERemove());
        }

        // Removes the end of the trail.
        private IEnumerator IERemove() {
            yield return new WaitForSeconds(m_FadeInterval);
            if (m_Trail.Count > 0) {
                m_Trail.RemoveAt(m_Trail.Count - 1);
            }
        }

        void OnEnable() {
            m_Trail = new List<Vector3>();
        }

    }
    
}
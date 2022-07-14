/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(LineRenderer))]
    public class Trail : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // Components.
        [HideInInspector] private LineRenderer m_LineRenderer;

        // Trail.
        [SerializeField] private float m_Width;
        [SerializeField] public float m_FadeInterval;
        [SerializeField, ReadOnly] public List<Vector3> m_Trail;

        #endregion

        /* --- Unity --- */
        #region Unity

        void Start() {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_Trail = new List<Vector3>();
        }

        void Update() {
            Capture();
            Render();
        }

        #endregion

        /* --- Trail --- */
        #region Trail

        private void Capture() {
            if (m_Trail.Count <= 0) {
                m_Trail.Insert(0, transform.position);
                StartCoroutine(IEFade());
            }
            else if ((m_Trail[0] - transform.position).magnitude > 0.05f) {
                m_Trail.Insert(0, transform.position);
                StartCoroutine(IEFade());
            }
            
        }

        private IEnumerator IEFade() {
            yield return new WaitForSeconds(m_FadeInterval);
            if (m_Trail.Count > 0) {
                m_Trail.RemoveAt(m_Trail.Count - 1);
            }
        }

        #endregion

        /* --- Rendering --- */
        #region Rendering

        void Render() {
            m_LineRenderer.startWidth = m_Width;
            m_LineRenderer.endWidth = 0f;
            m_LineRenderer.positionCount = m_Trail.Count;
            m_LineRenderer.SetPositions(m_Trail.ToArray());
        }

        #endregion
    }
    
}
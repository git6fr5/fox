/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class IceQueenIcicle : Projectile {

        /* --- Variables --- */
        #region Variables

        [HideInInspector] private Vector3 m_Origin;

        // Construction.
        [SerializeField] private AnimationCurve m_ConstructionCurve;
        [SerializeField] private float m_ConstructionDuration;
        [SerializeField, ReadOnly] private float m_ConstructionTicks;
        [HideInInspector] private float ConstructionRatio => m_ConstructionCurve.Evaluate(m_ConstructionTicks / m_ConstructionDuration);
        [HideInInspector] private bool Constructing => m_ConstructionTicks < m_ConstructionDuration;

        #endregion

        void Awake() {
            // Initialize the shield.
            m_Body.simulated = false;
            m_Body.gravityScale = 0f;
            m_Origin = transform.position;
            transform.localScale = new Vector3(0f, 0f, 0f);
            m_ConstructionTicks = 0f;
        }

        void LateUpdate() {
            if (Constructing) {
                transform.localScale = new Vector3(1f, 1f, 1f) * ConstructionRatio;
                Obstacle.Shake(transform, m_Origin, 0.1f * ConstructionRatio);
                bool finished = Timer.TickUp(ref m_ConstructionTicks, m_ConstructionDuration, Time.fixedDeltaTime);
                if (finished) {
                    Shatter();
                }
            }
        }

        public void Shatter() {
            //
        }

    }
}
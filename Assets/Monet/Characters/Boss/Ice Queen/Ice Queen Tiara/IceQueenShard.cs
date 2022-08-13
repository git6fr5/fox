/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class IceQueenShard : Projectile {

        [SerializeField] private float m_Radius;
        [SerializeField] private float m_Period;
        [SerializeField, ReadOnly] private float m_Ticks;

        void Awake() {
            // Initialize the shield.
            m_Body.simulated = false;
            m_Body.gravityScale = 5f;
        }

        public void SetOrbit(float radius, float offset, float period) {
            m_Period = period;
            m_Ticks = offset;
            m_Radius = radius;
        }

        void LateUpdate() {
            if (transform.parent != null) {
                float deltaTime = Time.deltaTime;
                Timer.CycleTicks(ref m_Ticks, m_Period, deltaTime);
                Obstacle.Cycle(transform, m_Ticks, m_Period, transform.parent.position, new Vector3(m_Radius, m_Radius));
            }
        }

    }
}
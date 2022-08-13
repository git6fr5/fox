/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class RespawnStation : Station {

        [SerializeField] private float m_Period;
        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField] private Vector2 m_Ellipse;
        [SerializeField] private Transform m_Orb;
        [SerializeField, ReadOnly] private Vector3 m_OrbOrigin;

        [SerializeField] private VisualEffect m_ActivationEffect;
        [SerializeField, ReadOnly] private float m_ActivationTicks = 0f;
        private static float ActivationBuffer = 0.5f;

        void Start() {
            m_OrbOrigin = m_Orb.position;
        }

        protected override void Activate(State state) {
            state.SetRespawn(this);
            Timer.CountdownTicks(ref m_ActivationTicks, true, ActivationBuffer, 0f);
            m_ActivationEffect.Play();
        }

        private void FixedUpdate() {
            Timer.CycleTicks(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(m_Orb, m_Ticks, m_Period, m_OrbOrigin, m_Ellipse);

            Timer.CountdownTicks(ref m_ActivationTicks, false, ActivationBuffer, Time.fixedDeltaTime);

        }
        
    }
}
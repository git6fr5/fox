/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SpitProjectile : Projectile {

        [HideInInspector] private float m_CachedY = 0f;
        [SerializeField] private float m_Period = 1f;
        [SerializeField] private Vector2 m_Ellipse = new Vector2(0f, 0.15f);
        [SerializeField, ReadOnly] private float m_Ticks;

        void Awake() {
            m_Body.simulated = false;
            m_Body.gravityScale = 0f;
            // m_Hitbox.enabled = false;
            m_CachedY = transform.position.y;
        }

        // Runs once every fixed interval.
        protected override void FixedUpdate() {
            Vector2 origin = new Vector2(transform.position.x, m_CachedY);
            Timer.CycleTicks(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, origin, m_Ellipse);
        }

        void LateUpdate() {
            // Timer.CycleTicks(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            // Obstacle.Cycle(m_Orb, m_Ticks, m_Period, m_OrbOrigin, m_Ellipse);
        }

    }

}
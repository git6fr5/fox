/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SlamProjectile : Projectile {

        void Awake() {
            m_Body.simulated = false;
            m_Body.gravityScale = 0f;
            transform.eulerAngles = Vector3.forward * Random.Range(0f, 360f);
            // m_Hitbox.enabled = false;
        }

        // Runs once every fixed interval.
        protected override void FixedUpdate() {
            transform.eulerAngles += Vector3.forward * Time.fixedDeltaTime * 120f;
        }

        void LateUpdate() {
            // Timer.CycleTicks(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            // Obstacle.Cycle(m_Orb, m_Ticks, m_Period, m_OrbOrigin, m_Ellipse);

            // float angle = Vector2.SignedAngle(m_Body.velocity, Vector2.up);
            // transform.eulerAngles = angle * Vector3.forward;

        }

    }

}
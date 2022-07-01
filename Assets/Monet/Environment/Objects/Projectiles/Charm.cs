/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Charm : Projectile {

        [SerializeField] private float m_Amplitude = 1f;
        [SerializeField] private float m_Period = 1f;
        [SerializeField, ReadOnly] private float m_PeriodTicks = 0f;

        void FixedUpdate() {

            // The math.
            // period = 2 pi / b
            // b = 2 pi / period

            // y = asin(bt)
            // dy / dt = a d sin(bt) / dt = a d sin(u) / du du / dt
            // dy / dt = a b cos(bt)

            Timer.UpdateTicks(ref m_PeriodTicks, true, Mathf.Infinity, Time.fixedDeltaTime);
            float a = m_Amplitude;
            float b = 2f * Mathf.PI / m_Period;
            float t = m_PeriodTicks;
            float dt = Time.fixedDeltaTime;
            float dy = a * b * Mathf.Cos(b * t);
            Vector3 unitY = Quaternion.Euler(0f, 0f, -90f) * m_Body.velocity.normalized;
            transform.position += unitY * dy * dt;

        }

    }
}
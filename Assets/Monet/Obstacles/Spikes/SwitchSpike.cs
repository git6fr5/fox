/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SwitchSpike : Spike, ISwitch {

        private static float Period = 2f;
        private static Vector2 Ellipse = new Vector2(0f, 0.5f);
        public Vector3 OffsetOrigin => m_Origin - Ellipse.y * Direction;

        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField, ReadOnly] protected bool m_Active = true;

        float x = 1f;

        public void Flip() {
            m_Active = !m_Active;
            x = Random.Range(0.75f, 1.25f);
        }

        void FixedUpdate() {
            Timer.UpdateTicks(ref m_Ticks, m_Active, Period / 4f, Time.fixedDeltaTime * x);
            Obstacle.Cycle(transform, m_Ticks, Period, OffsetOrigin, Ellipse);
        }

    }
}
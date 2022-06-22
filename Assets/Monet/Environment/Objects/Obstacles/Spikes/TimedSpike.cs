/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class TimedSpike : Spike {

        private static float Interval = 2f;

        // [SerializeField, ReadOnly] private float m_TickOffset;
        [SerializeField, ReadOnly] private float m_Ticks;
        
        // public void Init(int offset) {
        //     m_TickOffset = (float)offset * Interval / 2f;
        // }

        protected override void FixedUpdate() {
            float deltaTime = Time.deltaTime;
            m_Ticks += deltaTime;

            if (m_Ticks > Interval) {
                m_Ticks -= Interval;
                OnFlip();
            }

            base.FixedUpdate();
        }

    }
}
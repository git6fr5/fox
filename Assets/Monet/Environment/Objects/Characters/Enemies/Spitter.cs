/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// The most basic type of enemy that walks in
    /// between two points while waiting a little bit
    /// at the end of each point of its path.
    ///<summary>
    public class Spitter : Enemy {

        // Waiting.
        [SerializeField] private float m_SpitBuffer = 0.25f;
        [SerializeField] private float m_SpitTicks;
        public bool Spitting => Waiting && m_SpitTicks < m_SpitBuffer && !m_Spat;
        [SerializeField, ReadOnly] private bool m_Spat = false;
        
        // Runs once every frame to update the input.
        public override void OnUpdate() {
            if (Spitting) {
                AttackAction();
            }
            else if (Waiting) {
                WaitAction();
            }
            else {
                m_Spat = false;
                PathAction();
            }
        }

        // Runs once very fixed interval.
        protected override void FixedUpdate() {
            base.FixedUpdate();
            Timer.StartIf(ref m_SpitTicks, m_SpitBuffer, Waiting && m_SpitTicks == 0f && !m_Spat);
            bool spit = Timer.TickDownIf(ref m_SpitTicks, Time.fixedDeltaTime, Waiting);
            if (spit) {
                m_Spat = true;
            }
        }

    }
}
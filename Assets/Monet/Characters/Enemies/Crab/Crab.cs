/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// A basic type of enemy that walks in
    /// between two points while waiting a little bit
    /// at the end of each point of its path.
    ///<summary>
    public class Crab : Enemy {

        // Waiting.
        [SerializeField] private float m_SpitBuffer = 0.75f;
        [SerializeField] private float m_SpitTicks;
        [SerializeField] private int m_SpitCount;
        [SerializeField, ReadOnly] private bool m_Spat = false;
        public bool Spitting => IsSpitting();
        
        // Runs once every frame to update the input.
        public override void OnUpdate() {

            // float ticks = Game.Ticks % 3f;
            // bool spitting = !Waiting && ticks < 0.5f || (ticks > 0.75f && ticks < 1.25f);
            // bool waiting = !Waiting && ticks < 1.5f;

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

        public bool IsSpitting() {
            if (!Waiting || m_Spat) {
                return false;
            }
            else {
                float increment = m_SpitBuffer / (float)m_SpitCount;
                float ratio = m_SpitTicks % increment;
                if (ratio > 0.5f * increment) {
                    return true;
                }
                return false;
            }
        }

    }
}
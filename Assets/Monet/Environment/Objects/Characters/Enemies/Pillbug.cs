/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Pillbug : Enemy {

        public bool m_Wait;
        public float m_WaitTicks;
        public static float m_WaitBuffer = 1f;

        public override void OnUpdate() {
            if (m_Wait) {
                m_Direction = Vector2.zero;
                m_Jump = false; // bool
                m_HoldJump = false; // bool
                return;
            }

            m_Direction = (Vector2)(m_Path[m_PathIndex] - transform.position);
            m_Jump = false; // bool
            m_HoldJump = false; // bool
        }

        void FixedUpdate() {
            float distance = Mathf.Abs(m_Path[m_PathIndex].x - transform.position.x);
            float stepDistance = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) * Time.fixedDeltaTime;

            if (distance < stepDistance) {
                m_Wait = true;
            }

            Timer.CountdownTicks(ref m_WaitTicks, !m_Wait, m_WaitBuffer, Time.fixedDeltaTime);

            if (m_WaitTicks == 0f) {
                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
                m_Wait = false;
            }
        }

    }
}
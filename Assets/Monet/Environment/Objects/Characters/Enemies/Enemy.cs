/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Enemy : Input {

        [HideInInspector] protected Vector3 m_Origin;
        [SerializeField] protected Vector3[] m_Path = null;
        [SerializeField, ReadOnly] protected int m_PathIndex;

        // Waiting.
        public bool m_Wait;
        public float m_WaitTicks;
        public static float m_WaitBuffer = 1f;

        public void Init(Vector3[] path) {
            m_Origin = transform.position;
            m_Path = path;
            m_Wait = true;
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

        protected void PatrolAction() {
            if (m_Wait) {
                WaitAction();
                return;
            }
            PathAction();
        }

        protected void WaitAction() {
            m_Direction = Vector2.zero;
            m_Attack = false;
            m_Jump = false;
            m_HoldJump = false;
        }

        protected void PathAction() {
            m_Direction = (Vector2)(m_Path[m_PathIndex] - transform.position);
            m_Attack = false;
            m_Jump = false;
            m_HoldJump = false;
        }

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class BouncyPlatform : Platform {

        [SerializeField] private bool m_Bouncing;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_SinkSpeed;
        [SerializeField] private float m_SinkDistance = 0.5f;

        void LateUpdate() {
            m_Bouncing = m_PressedDown ? true : m_Bouncing;
        }

        void FixedUpdate() {
            Vector3 apex = m_Origin + Vector3.down * m_SinkDistance;
            
            if (m_Bouncing) {
                Obstacle.Move(transform, apex, m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);
                float distance = (transform.position - apex).magnitude;
                if (distance < Game.Physics.MovementPrecision) {
                    BounceBodies();
                    m_Bouncing = false;
                }
            }
            else {
                Obstacle.Move(transform, m_Origin, m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);
            }

        }

        private void BounceBodies() {
            for (int i = 0; i < m_CollisionContainer.Count; i++) {
                Character character = m_CollisionContainer[i].GetComponent<Character>();
                if (character != null) {
                    character.Body.velocity += new Vector2(0f, m_JumpSpeed);
                }

            }
        }

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// A useful script for checking whether things in the character are actually working.
    ///<summary>
    [System.Serializable]
    public class CharacterDebugger  {

        #region Variables.

        [HideInInspector] private Vector3 m_CachedPosition;
        [HideInInspector] private float m_CachedDirection;
        [SerializeField, ReadOnly] private float m_CalculatedSpeed;

        [HideInInspector] private Vector3 m_CachedGroundPosition;
        [HideInInspector] private bool m_CachedOnGround;
        [SerializeField, ReadOnly] private float m_JumpHeight;
        [SerializeField, ReadOnly] private float m_RisingTime;
        [SerializeField, ReadOnly] private float m_FallingTime;

        #endregion

        public void OnUpdate(Transform transform, Input input, Controller controller, State state, float dt) {
            DebugSpeed(transform.position, input.MoveDirection, dt);
            DebugJump(transform.position, controller.OnGround, controller.Rising, dt);
        }

        private void DebugSpeed(Vector3 currentPosition, float direction, float dt) {
            // Not working currently.
            if (direction == m_CachedDirection && direction != 0f) {
                float dx = Mathf.Abs(currentPosition.x - m_CachedPosition.x);
                m_CalculatedSpeed = dx / dt;
            }
            // Cache these values.
            m_CachedDirection = direction;
            m_CachedPosition = currentPosition;
        }

        private void DebugJump(Vector3 currentPosition, bool onGround, bool rising, float dt) {
            if (onGround) {
                m_CachedOnGround = true;
                m_CachedGroundPosition = currentPosition;
            }
            else {
                if (m_CachedOnGround) {
                    m_CachedOnGround = false;
                    m_RisingTime = 0f;
                    m_FallingTime = 0f;
                }

                if (rising) {
                    m_JumpHeight = currentPosition.y - m_CachedGroundPosition.y;
                    m_RisingTime += dt;
                }
                else {
                    m_FallingTime += dt;
                }

            }
        }

        public void Draw(Vector3 center, float radius) {            
            for (int i = -1; i <= 1; i++) {
                Vector3 offset = Vector3.down * radius + i * Vector3.left * radius / 1.5f;
                Gizmos.DrawWireSphere(center + offset, 0.05f);
            }
        }

    }

}
    
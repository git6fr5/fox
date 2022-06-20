/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class CharacterDebugger  {

        [HideInInspector] private Vector3 m_CachedPosition;
        [SerializeField, ReadOnly] private float m_CachedDirection;
        [SerializeField, ReadOnly] private float m_Speed;

        [HideInInspector] private Vector3 m_CachedGroundPosition;
        [HideInInspector] private bool m_CachedOnGround;
        [SerializeField, ReadOnly] private float m_JumpHeight;
        [SerializeField, ReadOnly] private float m_RisingTime;
        [SerializeField, ReadOnly] private float m_FallingTime;

        public void OnUpdate(Transform transform, Input input, Controller controller, State state, float dt) {
            DebugSpeed(transform.position, input.MoveDirection, dt);
            DebugJump(transform.position, controller.OnGround, controller.Rising, dt);
        }

        private void DebugSpeed(Vector3 currentPosition, float direction, float dt) {
            // Not working currently.
            if (direction == m_CachedDirection && direction != 0f) {
                float speed = Mathf.Abs(currentPosition.x - m_CachedPosition.x) / dt;
                m_Speed = speed;
            }
            
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

    }

}
    
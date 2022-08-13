/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SinkingPlatform : Platform {

        [SerializeField] private float m_SinkBuffer;
        [SerializeField, ReadOnly] private float m_SinkTicks;
        [SerializeField] private float m_RiseSpeed;
        [SerializeField] private float m_SinkSpeed;

        void FixedUpdate() {
            Timer.UpdateTicks(ref m_SinkTicks, m_PressedDown, m_SinkBuffer, Time.fixedDeltaTime);

            if (m_SinkTicks >= m_SinkBuffer) {
                Vector3 down = transform.position + Vector3.down;
                Obstacle.Move(transform, down, m_SinkSpeed, Time.fixedDeltaTime, m_CollisionContainer);
            }
            else if (m_SinkTicks == 0f) {
                Obstacle.Move(transform, m_Origin, m_RiseSpeed, Time.fixedDeltaTime, m_CollisionContainer);
            }
        }

    }
}
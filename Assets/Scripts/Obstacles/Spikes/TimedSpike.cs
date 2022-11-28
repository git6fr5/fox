/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class TimedSpike : Spike {

        private static float Period = 2f;
        private static float WaitDuration = 2f;
        private static Vector2 Ellipse = new Vector2(0f, 0.5f);

        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField, ReadOnly] private float m_WaitTicks;
        private bool m_Up = true;
        public bool Waiting => (m_Ticks == Period / 4f && m_Up) || (m_Ticks == 0f && !m_Up);
        public Vector3 OffsetOrigin => m_Origin - Ellipse.y * Direction;

        public void Init(int offset) {
            // 0 => peak down going up.
            // 1 => mid going up
            // 2 => peak up going down.
            // 3 => mid going down
            m_Up = offset < 2 ? true : false;
            m_Ticks = Mathf.Abs(Period / 8f - (float)offset * Period / 8f);
            if (offset % 2 == 0) {
                Timer.Start(ref m_WaitTicks, WaitDuration);
            }
            else {
                Timer.Start(ref m_WaitTicks, 0.01f);
            }
        }


        void FixedUpdate() {
            bool finished = Timer.TickDownIf(ref m_WaitTicks, Time.fixedDeltaTime, Waiting);
            m_Up = finished ? !m_Up : m_Up;
            Timer.StartIf(ref m_WaitTicks, WaitDuration, finished);
            
            Timer.TriangleTickDownIf(ref m_Ticks, Period / 4f, Time.fixedDeltaTime, m_Up);
            Obstacle.Cycle(transform, m_Ticks, Period, OffsetOrigin, Ellipse);
        }

    }

}
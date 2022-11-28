/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class MovingPlatform : Platform {

        [SerializeField] protected float m_Speed = 3f;
        
        protected override void Update() {
            m_Origin = transform.position;
            base.Update();
            Target();
        }

        private void FixedUpdate() {
            Obstacle.Move(transform, m_Path[m_PathIndex], m_Speed, Time.fixedDeltaTime, m_CollisionContainer);
        }

        // Sets the target for this platform.
        private void Target() {
            float distance = (m_Path[m_PathIndex] - transform.position).magnitude;
            if (distance < Game.Physics.MovementPrecision) {
                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
            }
        }
        
    }

}
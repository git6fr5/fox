/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class ElevatorPlatform : Platform, ISwitch {

        [SerializeField] protected float m_Speed = 3f;

        public void Flip() {
            m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
        }

        void FixedUpdate() {
            Obstacle.Move(transform, m_Path[m_PathIndex], m_Speed, Time.fixedDeltaTime, m_CollisionContainer);
        }
        
    }

}
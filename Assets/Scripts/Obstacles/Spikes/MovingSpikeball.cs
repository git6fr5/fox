/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class MovingSpikeball : Spikeball {

        [SerializeField] protected float m_Speed = 3f;

        [HideInInspector] protected Vector3[] m_Path = null;
        [SerializeField, ReadOnly] protected int m_PathIndex;

        public void Init(Vector3[] path) {
            m_Path = path;
            m_Rotation = 60f;
        }
        
        void Update() {
            Target();
        }

        private void FixedUpdate() {
            Obstacle.Move(transform, m_Path[m_PathIndex], m_Speed, Time.fixedDeltaTime);
            transform.eulerAngles += Vector3.forward * m_Rotation * Time.fixedDeltaTime;
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
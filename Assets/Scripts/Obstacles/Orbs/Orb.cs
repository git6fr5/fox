/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Utilites;
using Platformer.Obstacles;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public class Orb : MonoBehaviour {

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        [SerializeField] private float m_Period = 3f;
        [SerializeField] private Vector2 m_Ellipse = new Vector2(0f, 2f/16f);

        void Start() {
            m_Origin = transform.position;
        }

        void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
        }

    }
}
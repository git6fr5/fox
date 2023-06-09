/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Torch : MonoBehaviour {

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField] private Vector2 m_Ellipse;
        [SerializeField] private float m_Period;
        [SerializeField, ReadOnly] private float m_Ticks;

        void Start() {
            m_Origin = transform.position;
        }

        private void FixedUpdate() {
            Timer.CycleTicks(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
        }

    }
}
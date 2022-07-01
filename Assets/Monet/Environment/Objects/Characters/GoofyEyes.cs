/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class GoofyEyes : MonoBehaviour {

        [HideInInspector] private Transform m_Target;
        [HideInInspector] private Vector3 m_Offset;
        [SerializeField] private float m_Speed;
        [SerializeField] private float m_SpeedVariation;

        // Runs once before the first frame.
        void Start() {
            m_Target = transform.parent;
            m_Offset = transform.localPosition;
            transform.SetParent(null);
        }
        
        // Runs once every frame.
        void Update() {
            if (m_Target == null) {
                Destroy(gameObject);
                return;
            }

            float speed = m_Speed + UnityEngine.Random.Range(-m_SpeedVariation, m_SpeedVariation);
            Obstacle.Move(transform, m_Target.position + m_Offset, speed, Time.deltaTime, null);

        }
        
    }

}
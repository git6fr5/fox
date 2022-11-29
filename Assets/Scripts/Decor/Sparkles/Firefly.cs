/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;


namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class Firefly : SparkleController {
        
        // [SerializeField] protected float m_Speed = 0.75f; 

        // [SerializeField] private Vector2 m_Direction; 
        // [SerializeField] private float m_ThinkDuration; 

        // [SerializeField] private float m_DirectionTicks = 1f;
        // [SerializeField] private Vector2 m_Target;

        [SerializeField] private Vector2 m_Origin;

        // [SerializeField] private Sparkle m_SecondarySparkle;


        void Awake() {
            m_Origin = transform.position;
        }
        
        protected override bool CheckActivationCondition() {
            // bool enabled = true;
            // if (!enabled) {
            //     m_SecondarySparkle.Reset();
            // }
            // m_SecondarySparkle.enabled = enabled;
            return true;
        }

        // protected override void ExtraStuff(float dt) {
        //     transform.position += (Vector3)m_Direction * dt * m_Speed;

        //     bool changeDir = Timer.TickDown(ref m_DirectionTicks, dt);
        //     m_Direction += (m_Target - (Vector2)transform.position).normalized * dt;

        //     if (changeDir) {
        //         m_Direction = m_Direction.normalized;
        //         m_Target = m_Origin + Random.insideUnitCircle.normalized * 5f;
        //         m_DirectionTicks = m_ThinkDuration;
        //     }

        // }

    }

}
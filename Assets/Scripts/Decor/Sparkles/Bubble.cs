/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class Bubble : SparkleController {

        // The speed at which the bubble rises.
        [SerializeField] 
        protected float m_RiseSpeed = 0.75f;   
        
        // An adjustment factor that controls how the opacity affects the direction.
        [SerializeField] 
        protected float m_SpeedFactor = 5f;   

        protected override bool CheckActivationCondition() {
            return true;
        }

        protected override void UpdateSparkles(float dt) {
            base.UpdateSparkles(dt);
            SparkleAdjustments.OpacityMove2D(ref m_Sparkles, Vector3.up, m_SpeedFactor * m_RiseSpeed, (Vector3.up + Vector3.right).normalized, m_SpeedFactor, dt);
        }

    }

}
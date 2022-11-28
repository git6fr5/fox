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
    public class Rain : SparkleController {

        // The speed at which the rain falls.
        [SerializeField] 
        protected float m_FallSpeed = 0.75f; 

        // The speed at which the wind pushes the rain.
        [SerializeField] 
        private float m_WindSpeed = 0.4f;
        
        protected override bool CheckActivationCondition() {
            return true;
        }

        protected override void UpdateSparkles(float dt) {
            base.UpdateSparkles(dt);
            SparkleAdjustments.OpacityMove(ref m_Sparkles, Vector3.down, m_FallSpeed, Vector3.right, 0.4, dt);
        }

    }

}
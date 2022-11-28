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
    public class Smoke : SparkleController {

        // The speed at which this particle floats up.
        [SerializeField] 
        protected float m_MoveSpeed = 0.75f;   

        protected override bool CheckActivationCondition() {
            return true;
        }

        protected override void UpdateSparkles(float dt) {
            base.UpdateSparkles(dt);
            SparkleAdjustments.Move(ref m_Sparkles, Vector3.up, dt);
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Pillbug : Enemy {

        public override void OnUpdate() {
            Target();
            m_Direction = (Vector2)(m_Path[m_PathIndex] - transform.position);
            m_Jump = false; // bool
            m_HoldJump = false; // bool
        }

        // Sets the target for this enemy.
        protected void Target() {
            float distance = (m_Path[m_PathIndex] - transform.position).magnitude;
            if (distance < Game.Physics.MovementPrecision) {
                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
            }
        }

    }
}
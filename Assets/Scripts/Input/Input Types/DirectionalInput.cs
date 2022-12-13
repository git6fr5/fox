/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Input;

namespace Platformer.Input {

    ///<summary>
    /// Processes all the directional input information.
    ///<summary>
    [System.Serializable]
    public class DirectionalInput {

        // The actual inputted direction,
        // Whether this is a joystick, keypad or anything else.
        [SerializeField, ReadOnly] 
        private Vector2 m_Vector = new Vector2(0f, 0f);

        // The normalized direction.
        public Vector2 Normal => m_Vector.normalized;

        // The horizontal direction.
        public float Horizontal => m_Vector.x;

        // The vertical direction.
        public float Vertical => m_Vector.y;

        // Updates this directional input.
        public void OnUpdate(Vector2 vector) {
            m_Vector = vector;
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character.Input;

namespace Platformer.Character.Input {

    ///<summary>
    /// Processes all the directional input information.
    ///<summary>
    public class DirectionalInput {

        // The actual inputted direction,
        // Whether this is a joystick, keypad or anything else.
        [SerializeField] private Vector2 m_Direction;
        [SerializeField] private float m_Facing;

        // The useable information from the inputted direction.
        public float Facing => m_Facing;
        public float Move => m_Direction.x;
        public Vector2 Fly => m_Direction.normalized;

        // Updates this directional input.
        public void OnUpdate(Vector2 vector) {
            m_Direction = vector;
            m_Facing = m_Direction.x != 0f ? m_Direction.x : Facing;
        }

    }

}
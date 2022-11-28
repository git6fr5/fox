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
        [SerializeField] private bool m_LockFacing;

        // The useable information from the inputted direction.
        public float Facing => m_Facing;
        public float Move => m_Direction.x;
        public float Climb => m_Direction.y;
        public Vector2 Fly => m_Direction.normalized;

        // Updates this directional input.
        public void OnUpdate(Vector2 vector) {
            m_Direction = vector;
            if (!m_LockFacing) {
                m_Facing = m_Direction.x != 0f ? m_Direction.x : Facing;
            }
        }
        
        // Forcibly flips the facing direction of this.
        public void ForceFacing(float direction) {
            m_Facing = Mathf.Sign(direction);
        }

        // Forcibly locks the direction being faced.
        public void LockFacing(bool lockFacing) {
            m_LockFacing = lockFacing;
        }

    }

}
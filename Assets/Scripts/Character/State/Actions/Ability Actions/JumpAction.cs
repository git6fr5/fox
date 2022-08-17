/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An ability that shoots the player into the air.
    ///<summary>
    [System.Serializable]
    public class JumpAction : AbilityAction {

        #region Variables

        // The setting for this jump.
        [SerializeField] private float m_Height;
        [SerializeField] private float m_RisingTime;
        [SerializeField] private float m_FallingTime;
        // The calculated settings for this jump.
        [SerializeField, ReadOnly] private float m_Speed;
        [SerializeField, ReadOnly] private float m_Weight;
        public float Weight => m_Weight;
        [SerializeField, ReadOnly] private float m_Sink;
        public float Sink => m_Sink;

        // Allows a little leeway for when this character is no longer
        // in contact with the ground.
        [SerializeField] private float m_CoyoteBuffer = 0f;
        [SerializeField, ReadOnly] private float m_CoyoteTicks = 0f;
        public float CoyoteTicks => m_CoyoteTicks; 

        #endregion

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }
            
            // Chain the dash actions.
            body.Move(Vector2.up * Game.Physics.MovementPrecision);
            body.ClampFallSpeed(0f);
            body.AddVelocity(Vector2.up * m_Speed);
            // body.SetWeight(m_Weight);

            // Clear the inputs.
            input.Action0.ClearPressBuffer();

            // Set this on cooldown.
            m_CoyoteTicks = 0f;
             
        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }
            
            RefreshJumpSettings(ref m_Speed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);
            if (!state.OnGround) {
                Timer.TickDown(ref m_CoyoteTicks, dt);
            }
            else {
                Timer.Start(ref m_CoyoteTicks, m_CoyoteBuffer);
            }
            m_Refreshed = state.OnGround || m_CoyoteTicks > 0f;
        }

        // Calculates the speed and weight of the jump.
        public static void RefreshJumpSettings(ref float v, ref float w, ref float s, float h, float t_r, float t_f) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
            s = (t_f * t_f) * w * Mathf.Abs(UnityEngine.Physics2D.gravity.y) / (2f * h);
            s = 1f / s;
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Refreshed;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action0.Pressed;
        }

        

    }
}
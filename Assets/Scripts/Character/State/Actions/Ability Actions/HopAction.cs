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
    /// An ability that near-instantly moves the character.
    ///<summary>
    [System.Serializable]
    public class HopAction : AbilityAction {

        #region Variables

        // Checks whether the external activation conditions for this
        // ability have been fulfilled.
        [SerializeField, ReadOnly] private bool m_Refreshed;
        public bool Refreshed => m_Refreshed;

        // The setting for this jump.
        [SerializeField] private float m_Height;
        [SerializeField] private float m_RisingTime;
        // The calculated settings for this jump.
        [SerializeField, ReadOnly] private float m_Speed;
        [SerializeField, ReadOnly] private float m_Weight;

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private float m_Charge;
        public float Charge => m_Charge;
        [SerializeField] private float m_MaxCharge;

        // Tracks the timeline of the dash.

        #endregion

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {

            // Chain the dash actions.
            state.OverrideFall(true);
            state.OverrideMovement(false);

            body.SetVelocity(m_Speed * m_Charge / m_MaxCharge * Vector2.up);
            body.SetWeight(m_Weight);

            // Clear the inputs.
            input.Action2.ClearReleaseBuffer();

            // Set this on cooldown.
            m_Charge = 0f;
            m_Refreshed = false;

        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            m_Refreshed = state.OnGround ? true : m_Refreshed;
            RefreshHopSettings(ref m_Speed, ref m_Weight, m_Height, m_RisingTime);
            
            // Charge the hop.
            if (!state.Disabled && m_Refreshed && input.Action2.Held) {
                bool finished = Timer.TickUp(ref m_Charge, m_MaxCharge, dt);
                state.OverrideMovement(true);
                state.OverrideFall(true);
                body.SetVelocity(Vector2.zero);
                body.SetWeight(0f);
            }

            if (!state.Disabled && !body.Rising() && !m_Refreshed) {
                state.OverrideFall(false);
            }

        }

        // Calculates the speed and weight of the jump.
        public static void RefreshHopSettings(ref float v, ref float w, float h, float t_r) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Refreshed;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action2.Released;
        }

        

    }
}
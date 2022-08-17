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
    public class DashAction : AbilityAction {

        #region Variables

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private bool m_PreDashing;
        public bool Predashing => m_PreDashing;
        [SerializeField, ReadOnly] private bool m_Dashing;
        public bool Dashing => m_Dashing;
        [SerializeField, ReadOnly] private Vector2 m_CachedDirection;
        [SerializeField, ReadOnly] private Vector2 m_CachedVelocity;

        // Tracks the timeline of the dash.
        [SerializeField, ReadOnly] private float m_DashTicks;
        [SerializeField] private float m_PreDashBuffer;
        [SerializeField] private float m_DashBuffer;
        [SerializeField] private float m_CooldownBufferTicks;
        public float Cooldown => m_PreDashBuffer + m_DashBuffer + m_CooldownBufferTicks;
        public bool EndPreDash => m_PreDashing && m_DashTicks <= Cooldown - m_PreDashBuffer;
        public bool EndDash => m_Dashing && m_DashTicks <= Cooldown - m_PreDashBuffer - m_DashBuffer;
        public bool HalfwayFinished => m_Dashing && m_DashTicks <= Cooldown - m_PreDashBuffer - m_DashBuffer / 2f;

        // The distance covered by the dash.
        [SerializeField] private float m_DashDistance;
        private float DashSpeed => m_DashDistance / m_DashBuffer;

        #endregion

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            // Chain the dash actions.
            state.Disable(Cooldown - m_CooldownBufferTicks);
            state.OverrideMovement(true);
            state.OverrideFall(true);

            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);
            m_CachedDirection = new Vector2(input.Direction.Facing, 0f);
            // input.Direction.Fly != Vector2.zero ? input.Direction.Fly : 
            m_CachedVelocity = body.velocity;

            // Clear the inputs.
            input.Action1.ClearPressBuffer();

            // Set this on cooldown.
            m_PreDashing = true;
            Timer.Start(ref m_DashTicks, Cooldown);
            m_Refreshed = false;
        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            m_Refreshed = state.OnGround ? true : m_Refreshed;
            Timer.TickDown(ref m_DashTicks, dt);
            
            // When ending the predash, start the dash.
            if (EndPreDash) {
                body.SetVelocity(m_CachedDirection * DashSpeed);
                m_PreDashing = false;
                m_Dashing = true;
            }
            
            // When ending the dash, halt the body by alot.
            if (EndDash) {
                body.SetVelocity(m_CachedVelocity);
                state.OverrideMovement(false);
                state.OverrideFall(false);
                m_Dashing = false;
            }
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Refreshed && m_DashTicks == 0f;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Pressed;
        }

        

    }
}
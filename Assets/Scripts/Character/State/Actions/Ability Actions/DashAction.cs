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

        // Checks whether the external activation conditions for this
        // ability have been fulfilled.
        [SerializeField] private bool m_Refreshed;

        // Tracks whether the dash has started.
        [SerializeField] private bool m_PreDashing;
        [SerializeField] private bool m_Dashing;
        [SerializeField] private float m_CachedDirection;

        // Tracks the timeline of the dash.
        [SerializeField] private float m_DashTicks;
        [SerializeField] private float m_PreDashBuffer;
        [SerializeField] private float m_DashBuffer;
        [SerializeField] private float m_CooldownBufferTicks;
        public float Cooldown => m_PreDashBuffer + m_DashBuffer + m_CooldownBufferTicks;
        public bool EndPreDash => m_PreDashing && m_DashTicks <= Cooldown - m_PreDashBuffer;
        public bool EndDash => m_Dashing && m_DashTicks <= 0f;

        // The distance covered by the dash.
        [SerializeField] private float m_DashDistance;
        private float DashSpeed => m_DashDistance / m_DashBuffer;

        #endregion

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            // Chain the dash actions.
            state.Disable(Cooldown - m_CooldownBufferTicks);
            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);
            m_CachedDirection = input.Direction.Facing;

            // Clear the inputs.
            input.Action1.ClearReleaseBuffer();

            // Set this on cooldown.
            m_PreDashing = true;
            Timer.Start(ref m_DashTicks, Cooldown);
            m_Refreshed = false;
        }

        // Refreshes the settings for this ability every interval.
        public override void Refresh(Rigidbody2D body, CharacterState state, float dt) {
            m_Refreshed = state.OnGround ? true : m_Refreshed;
            Timer.TickDown(ref m_DashTicks, dt);
            
            // When ending the predash, start the dash.
            if (EndPreDash) {
                body.SetVelocity(new Vector2(m_CachedDirection, 0f) * DashSpeed);
                m_PreDashing = false;
                m_Dashing = true;
            }
            
            // When ending the dash, halt the body by alot.
            if (EndDash) {
                body.SetVelocity(body.velocity * 0.25f);
                m_Dashing = false;
            }
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_Refreshed && m_DashTicks == 0f && state.OnGround;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Released;
        }

        

    }
}
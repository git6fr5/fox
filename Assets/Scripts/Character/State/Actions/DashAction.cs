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
    public class DashAction : Action {

        #region Variables

        // The different stages of the dash.
        [SerializeField, ReadOnly] private bool m_Predashing;
        public bool Predashing => m_Predashing;
        [SerializeField, ReadOnly] private bool m_Dashing;
        public bool Dashing => m_Dashing;

        // The durations of the different stages of the dash.
        [SerializeField] private float m_PreDashBuffer;
        [SerializeField] private float m_DashBuffer;
        [SerializeField] private float m_CooldownBufferTicks;

        // Tracks the timeline of the dash.
        [SerializeField, ReadOnly] private float m_DashTicks;
        public float Cooldown => m_PreDashBuffer + m_DashBuffer + m_CooldownBufferTicks;
        public bool EndPredash => m_Predashing && m_DashTicks <= Cooldown - m_PreDashBuffer;
        public bool EndDash => m_Dashing && m_DashTicks <= Cooldown - m_PreDashBuffer - m_DashBuffer;
        public bool HalfwayFinished => m_Dashing && m_DashTicks <= Cooldown - m_PreDashBuffer - m_DashBuffer / 2f;

        // The distance covered by the dash.
        [SerializeField] private float m_DashDistance;
        private float DashSpeed => m_DashDistance / m_DashBuffer;

        // The sound for this ability.
        [SerializeField] private AudioClip m_DashSound;

        #endregion

        // Enable/disable this ability.
        public override void Enable(CharacterState state, bool enable) {
            if (!enable) {
                OnEndDash(m_DashTicks, m_Dashing, state, state.Body, state.Input);
            }
            base.Enable(state, enable);
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            OnStartPredash(body, input, state);
            input.Action1.ClearPressBuffer();
            m_Refreshed = false;

        }

        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            // Refreshing.
            m_Refreshed = state.OnGround ? true : m_Refreshed;
            
            // Chaining the events of the dash.
            Timer.TickDown(ref m_DashTicks, dt);
            if (EndPredash) {
                OnStartDash(ref m_Predashing, ref m_Dashing, DashSpeed, body, input);
            }
            else if (EndDash) {
                OnEndDash(ref m_DashTicks, ref m_Dashing, state.Move.Speed, body, input);
            }

            // Effects that occur while dashing.
            if (m_Predashing) {
            }
            else if (m_Dashing) {
            }

        }

        private void OnStartPredash(Rigidbody2D body, InputSystem input, CharacterState state) {
            // Stop the body momentarily.
            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);

            // Disable the body.
            state.Default.Disable();

            // Track the timeline of the dash.
            m_Predashing = true;
            m_Dashing = false;
            Timer.Start(ref m_DashTicks, Cooldown);
        }

        // Starts the dash.
        private static void OnStartDash(float dashSpeed, Rigidbody2D body, InputSystem input) {
            // Shoot the body in the direction being faced.
            body.SetVelocity(input.Direction.Facing * dashSpeed);
            
            // Track the timeline of the dash.
            m_Predashing = false;
            m_Dashing = true;

            // Give the player feedback.
            SoundManager.PlaySound(m_DashSound, 0.15f);

        }

        // Ends the dash.
        private static void OnEndDash(float moveSpeed, Rigidbody2D body, InputSystem input) {
            
            // Adjust the speed for the end of the dash.
            if (m_Dashing) {
                float inputDirection = input.Direction.Move;
                float direction = Mathf.Sign(body.velocity.x);
                if (inputDirection == direction) {
                    body.SetVelocity(direction * moveSpeed * Vector2.right);
                }
                else {
                    body.SetVelocity(Vector2.zero);
                }
            }

            // Re-enable the body.
            state.Default.Enable();

            // Track the timeline of the dash.
            m_Predashing = false;
            m_Dashing = false;
            m_DashTicks = 0f;

        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            return m_Refreshed && m_DashTicks == 0f;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Pressed;
        }
        

    }
}


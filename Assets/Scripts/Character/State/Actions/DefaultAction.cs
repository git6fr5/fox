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
    public class DefaultAction : Action {

        #region Variables

        // Seperating these two functionalities.
        [SerializeField] private bool m_MoveEnabled;
        [SerializeField] private bool m_FallEnabled;

        // The movement settings.
        [SerializeField] private float m_Speed;
        public float Speed => m_Speed;
        [SerializeField] private float m_Acceleration;

        // The setting for this jump.
        [SerializeField] private float m_Height;
        [SerializeField] private float m_RisingTime;
        [SerializeField] private float m_FallingTime;
        [SerializeField] private float m_MaxFallSpeed = 25f;
        // The calculated settings for this jump.
        [SerializeField, ReadOnly] private float m_JumpSpeed;
        [SerializeField, ReadOnly] private float m_Weight;
        public float Weight => m_Weight;
        [SerializeField, ReadOnly] private float m_Sink;
        public float Sink => m_Sink;

        // Allows a little leeway for when this character is no longer
        // in contact with the ground.
        [SerializeField] private float m_CoyoteBuffer = 0f;
        [SerializeField, ReadOnly] private float m_CoyoteTicks = 0f;
        public float CoyoteTicks => m_CoyoteTicks; 

        // Holds the player at the apex of their jump for a brief moment longer.
        [SerializeField] private float m_AntiGravityFactor;
        [SerializeField] private float m_AntiGravityBuffer;
        [SerializeField, ReadOnly] private float m_AntiGravityTicks;

        #endregion

        // Enable/disable this ability.
        public override void Enable(CharacterState state, bool enable) {
            m_MoveEnabled = enable;
            m_FallEnabled = enable;
            m_Enabled = enable;
        }

        // Enable everything.
        public void Enable() {
            m_MoveEnabled = true;
            m_FallEnabled = true;
            m_Enabled = true;
        }

        // Enable/disable the movement and falling seperately.
        public void Enable(bool moveEnable, bool fallEnable) {
            m_MoveEnabled = moveEnable;
            m_FallEnabled = fallEnable;
            m_Enabled = fallEnable || moveEnable;
        }

        // Disable the default action.
        public void Disable() {
            m_MoveEnabled = false;
            m_FallEnabled = false;
            m_Enabled = false;
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            OnJump(body);
            input.Action0.ClearPressBuffer();
            m_Refreshed = false;
            
        }

        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }
            
            // Refreshing.
            m_Refreshed = state.OnGround || m_CoyoteTicks > 0f;

            // Tick the coyote timer.
            Timer.TickDownIfElseReset(ref m_CoyoteTicks, m_CoyoteBuffer, dt, !state.OnGround);
            if (m_MoveEnabled) { 
                WhileMoving(body, input, state, dt); 
            }
            if (m_FallEnabled) { 
                WhileFalling(body, input, state, dt); 
            }
            
        }

        private void OnJump(Rigidbody2D body) {
            // Refresh the jump settings.
            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            body.Move(Vector2.up * Game.Physics.MovementPrecision);
            body.ClampFallSpeed(0f);
            body.AddVelocity(Vector2.up * m_JumpSpeed);

            // Reset the coyote ticks.
            m_CoyoteTicks = 0f;
        }

        // Process the physics of this action.
        private void WhileMoving(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            // Cache the target and current velocities.
            float targetSpeed = input.Direction.Move * m_Speed;
            float currentSpeed = body.velocity.x;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                body.velocity = new Vector2(targetSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(currentSpeed + deltaSpeed, body.velocity.y);
            }
            
        }

        // Not really falling, but rather "while default grav acting on this body"
        private void WhileFalling(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            // Set the weight to the default.
            body.gravityScale = Game.Physics.GravityScale;

            // If the body is not on the ground.
            if (!state.OnGround) { 
                // And it is rising.
                if (body.Rising()) {

                    // Multiply it by its weight.
                    body.gravityScale *= state.Jump.Weight;
                    Timer.Start(ref m_AntiGravityTicks, m_AntiGravityBuffer);

                    // If not holding jump, then rapidly slow the rising body.
                    if (!input.Action0.Held) {
                        body.SetVelocity(new Vector2(body.velocity.x, body.velocity.y * 0.9f));
                    }
                    
                }
                else {
                    // If it is falling, also multiply the sink weight.
                    body.gravityScale *= (state.Jump.Weight * state.Jump.Sink);

                    // If it is still at its apex, factor this in.
                    if (!body.Rising() && m_AntiGravityTicks > 0f) {
                        body.gravityScale *= m_AntiGravityFactor;
                        Timer.TickDown(ref m_AntiGravityTicks, dt);
                    }

                    // If the coyote timer is still ticking, fall slower.
                    if (state.Jump.CoyoteTicks > 0f) {
                        body.gravityScale *= 0.5f;
                    }

                }

            }

            // Clamp the fall speed at a given value.
            body.ClampFallSpeed(m_MaxFallSpeed);
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
            return m_Refreshed;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action0.Pressed;
        }
        
    }

}


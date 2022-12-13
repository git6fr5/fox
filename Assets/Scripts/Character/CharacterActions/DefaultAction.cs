/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Input;
using Platformer.Physics;
using Platformer.Utilities;
using Platformer.Character;
using Platformer.Character.Actions;

// Definitions
using Game = Platformer.GameManager;

namespace Platformer.Character.Actions {

    ///<summary>
    /// The default ability for controlling the character.
    ///<summary>
    [System.Serializable]
    public class DefaultAction : CharacterAction {

        #region Variables

        /* --- Constant Variables --- */

        // The factor that slows down rising if not holding the input.
        protected const float RISE_CLAMP = 0.9f;

        // The amount of time before noticing we're falling.
        protected const float COYOTE_BUFFER = 0.08f;

        // The amount of hang time.
        protected const float HANG_BUFFER = 0.08f;

        // Holds the player at the apex of their jump for a brief moment longer.
        protected const float HANG_FACTOR = 0.95f;

        /* --- Member Variables --- */

        // Whether the character is using the default movement.
        [SerializeField] 
        public bool moveEnabled = true;

        // Whether the character is using the default falling.
        [SerializeField] 
        public bool fallEnabled = true;

        // The default speed the character moves at.
        [SerializeField] 
        private float m_Speed = 5f;

        // The default acceleration of the character.
        [SerializeField] 
        private float m_Acceleration = 100f;

        // The default jump height of the character.
        [SerializeField] 
        private float m_Height = 3f;

        // The default time taken to reach the apex of the jump.
        [SerializeField] 
        private float m_RisingTime = 1f;

        // The default time taken to fall from the apex of the jump.
        [SerializeField] 
        private float m_FallingTime = 0.75f;

        // The maximum speed with which this character can fall.
        [SerializeField] 
        private float m_MaxFallSpeed = 25f;

        // The calculated jump speed based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_JumpSpeed = 0f;

        // The calculated weight based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_Weight = 0f;

        // The calculated sink factor based on the height, rising and falling time.
        // The sink is the factor applied to the weight when falling.
        [SerializeField, ReadOnly] 
        private float m_Sink = 0f;

        // Tracks how long the character has not been on the ground.
        [SerializeField, ReadOnly] 
        private Timer m_CoyoteTimer = new Timer(0f, COYOTE_BUFFER);

        // Tracks how long its been since the character reached the apex.
        [SerializeField, ReadOnly] 
        private Timer m_HangTimer = new Timer(0f, HANG_BUFFER);

        #endregion

        // Runs once every frame to check the inputs for this ability.
        public override void InputUpdate(CharacterController controller) {
            if (!m_Enabled) { return; }

            if (controller.CharacterInput.Actions[0].Pressed && m_Refreshed) {
                // The character should jump.
                OnJump(controller);

                // Release the input and reset the refresh.
                controller.CharacterInput.Actions[0].Clear(ActionInput.State.Press);
                m_Refreshed = false;
            }
            
        }

        // 
        public override void PhysicsUpdate(CharacterController controller, float dt) {
            if (!m_Enabled) { return; }
            
            // Refreshing.
            m_Refreshed = controller.OnGround || m_CoyoteTimer.Active;

            // Tick the coyote timer.
            m_CoyoteTimer.TickDownIfElseReset(dt, !controller.OnGround);
            if (moveEnabled) { 
                WhileMoving(controller, dt); 
            }
            if (fallEnabled) { 
                WhileFalling(controller, dt); 
            }
            
        }

        private void OnJump(CharacterController controller) {
            // Refresh the jump settings.
            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            controller.Move(Vector2.up * Game.Physics.MovementPrecision);
            controller.ClampFallSpeed(0f);
            controller.AddVelocity(Vector2.up * m_JumpSpeed);

            // Reset the coyote ticks.
            m_CoyoteTimer.Stop();
        }

        // Process the physics of this action.
        private void WhileMoving(CharacterController controller, float dt) {
            // Cache the target and current velocities.
            float targetSpeed = controller.CharacterInput.Direction.Horizontal * m_Speed;
            float currentSpeed = controller.CharacterBody.velocity.x;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            Vector2 velocity = new Vector2(currentSpeed + deltaSpeed, controller.CharacterBody.velocity.y);
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                velocity = new Vector2(targetSpeed, controller.CharacterBody.velocity.y);
            }

            // Set the velocity.
            controller.SetVelocity(velocity);
            
        }

        // Not really falling, but rather "while default grav acting on this body"
        private void WhileFalling(CharacterController controller, float dt) {
            // Set the weight to the default.
            float weight = 1f;

            // If the body is not on the ground.
            if (!controller.OnGround) { 
                // And it is rising.
                if (controller.Rising) {

                    // Multiply it by its weight.
                    weight *= m_Weight;
                    m_HangTimer.Start();

                    // If not holding jump, then rapidly slow the rising body.
                    if (!controller.CharacterInput.Actions[0].Held) {
                        controller.SetVelocity(new Vector2(controller.CharacterBody.velocity.x, controller.CharacterBody.velocity.y * RISE_CLAMP));
                    }
                    
                }
                else {
                    // If it is falling, also multiply the sink weight.
                    weight *= (m_Weight * m_Sink);

                    // If it is still at its apex, factor this in.
                    if (m_HangTimer.Active) {
                        weight *= HANG_FACTOR;
                        m_HangTimer.TickDown(dt);
                    }

                    // If the coyote timer is still ticking, fall slower.
                    if (m_CoyoteTimer.Active) {
                        weight *= 0.5f;
                    }

                }

            }

            // Set the weight.
            controller.SetWeight(weight);

            // Clamp the fall speed at a given value.
            controller.ClampFallSpeed(m_MaxFallSpeed);
        }

        // Calculates the speed and weight of the jump.
        public static void RefreshJumpSettings(ref float v, ref float w, ref float s, float h, float t_r, float t_f) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
            s = (t_f * t_f) * w * Mathf.Abs(UnityEngine.Physics2D.gravity.y) / (2f * h);
            s = 1f / s;
        }

    }

}


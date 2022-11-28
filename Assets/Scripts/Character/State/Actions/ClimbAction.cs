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
    /// 
    ///<summary>
    [System.Serializable]
    public class ClimbAction : Action {

        #region Variables

        public bool Climbing => m_Enabled && m_ClimbTicks > 0f && !m_WallJumping;

        // Tracks whether the dash has started.
        [SerializeField, ReadOnly] private float m_ClimbTicks;
        [SerializeField] private float m_Duration;

        [SerializeField] private bool m_WallJumping;
        public bool WallJumping => m_Enabled && m_WallJumping;

        // Allows a little leeway for when this character is no longer
        // in contact with the wall.
        [SerializeField] private float m_CoyoteBuffer = 0f;
        [SerializeField, ReadOnly] private float m_CoyoteTicks = 0f;
        public float CoyoteTicks => m_CoyoteTicks; 

        [SerializeField] private float m_CachedDirection;

        [SerializeField] private AudioClip m_StartJumpSound;
        [SerializeField] private AudioClip m_StartClimbSound;
        [SerializeField] private AudioClip m_EndJumpSound;
        [SerializeField] private AudioClip m_WallJumpingSound;


        [SerializeField] private float m_WallJumpSpeed = 12.5f;

        #endregion

        public override void Enable(CharacterState character, bool enable) {
            base.Enable(character, enable);
            if (enable) {
                if (character.Input.Action1.Held) {
                    Activate(character.Body, character.Input, character);
                }
                m_CoyoteTicks = 0f;
            }
            if (!enable) {
                m_WallJumping = false;
                character.Input.Direction.LockFacing(false);
                m_ClimbTicks = 0f;
            }
        }

        // When this ability is activated.
        public override void Activate(Rigidbody2D body, InputSystem input, CharacterState state) {
            if (!m_Enabled) { return; }

            if (input.Action0.Pressed && Climbing) {
                OnStartWallJump(body, input, state);
                input.Action0.ClearPressBuffer();
            }

            if (input.Action1.Pressed && m_Refreshed) {
                OnStartClimb(body, input, state);
                input.Action1.ClearPressBuffer();
                m_Refreshed = false;
            }

        }

        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (!m_Enabled) { return; }

            Timer.TickDownIfElseReset(ref m_CoyoteTicks, m_CoyoteBuffer, dt, !state.FacingWall);
            m_Refreshed = (state.OnGround && state.FacingWall || m_CoyoteTicks > 0f);

            if (!input.Action1.Held) {
                if (m_Climbing) {
                    OnEndClimb(input, state);
                }
                if (m_WallJumping) {
                    OnEndWallJump(input);
                }
                return;
            }

            if (m_WallJumping) {
                WhileWallJumping(body);
                if (state.FacingWall) {
                    OnAttachToWall();
                }
            }

            if (m_Climbing) {
                WhileClimbing(body, input);
                bool finished = Timer.TickDown(ref m_ClimbTicks, dt);
                if (finished || (!state.FacingWall && m_CoyoteTicks == 0f)) {
                    OnEndClimb(state);
                }
            }

        }

        private void OnStartClimb(Rigidbody2D body, InputSystem input, CharacterState state) {
            
            state.Default.Disable();

            body.SetVelocity(Vector2.zero);
            body.SetWeight(0f);

            // Set this on cooldown.
            Timer.Start(ref m_ClimbTicks, m_Duration);
            SoundManager.PlaySound(m_StartClimbSound, 0.1f);

        }

        private void WhileClimbing(Rigidbody2D body, InputSystem input) {
            // Cache the target and current velocities.
            float targetSpeed = input.Direction.Climb * m_Speed;
            float currentSpeed = body.velocity.y;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                body.velocity = new Vector2(0f, targetSpeed);
            }
            else {
                body.velocity = new Vector2(0f, currentSpeed + deltaSpeed);
            }

        }

        private void OnEndClimb(InputSystem input, CharacterState state) {
            state.Default.Enable();
            m_ClimbTicks = 0f;
        }

        private void OnStartWallJump(Rigidbody2D body, InputSystem input, CharacterState state) {
            // Disable the state.
            state.Default.Disable();

            // Get the direction of the wall jump.
            if (state.FacingWall) {
                input.Direction.ForceFacing(-input.Direction.Facing);
            }
            input.Direction.LockFacing(true);

            // Set the body.
            Vector2 direction = input.Direction.Facing * Vector2.right;
            body.Move(direction * Game.Physics.MovementPrecision);
            body.SetVelocity(m_WallJumpSpeed * direction);
            body.SetWeight(0f);

            // Track the state.
            m_WallJumping = true;
            m_Climbing = false;

            // Give the player feedback.
            SoundManager.PlaySound(m_StartJumpSound, 0.1f);

        }

        private void WhileWallJumping(Rigidbody2D body) {
            SoundManager.PlaySound(m_WallJumpingSound, 0.1f);
        }

        private void OnEndWallJump(InputSystem input) {
            input.Direction.LockFacing(false);
            if (m_WallJumping) {
                SoundManager.PlaySound(m_EndJumpSound, 0.1f);
                m_WallJumping = false;
            }
        }

        // Checks the state for whether this ability can be activated.
        public override bool CheckState(CharacterState state) {
            if (state.Disabled) { return false; }
            return m_CanClimb;
        }

        // Checks the input for whether this ability should be activated.
        public override bool CheckInput(InputSystem input) {
            return input.Action1.Pressed || input.Action0.Pressed;
        }       

    }

}


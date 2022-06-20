/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    /// <summary>
    /// Controls the actions of a character.
    /// Modeled for a platforming game.
    /// </summary>
    [System.Serializable]
    public class Controller {

        // Knockback.
        [SerializeField, ReadOnly] protected float m_KnockbackTicks;
        public bool Knockedback => m_KnockbackTicks > 0f;

        // Coyote Time.
        [SerializeField] private float m_CoyoteBuffer = 0f;
        [SerializeField, ReadOnly] private float m_CoyoteTicks = 0f;
        public float CoyoteTicks => m_CoyoteTicks; 

        // Anti Gravity Buffer.
        [SerializeField] private float m_AntiGravityFactor = 0f;
        [SerializeField] private float m_AntiGravityBuffer = 0f;
        [SerializeField, ReadOnly] private float m_AntiGravityTicks = 0f;
        public float AntiGravTicks => m_AntiGravityTicks; 

        // Checks.
        [SerializeField, ReadOnly] private bool m_OnGround = false;
        public bool OnGround => m_OnGround;
        [SerializeField, ReadOnly] private bool m_Rising = false;
        public bool Rising => m_Rising;

        /* --- Initialization --- */
        #region Initialization

        public void Init() {

        }
        

        #endregion

        /* --- Process --- */
        #region Unity 

        public void OnUpdate(Rigidbody2D body, CircleCollider2D collisionFrame, Input input, State state) {        
            // Checks.
            PhysicsCheck.OnGround(body.position + collisionFrame.offset, collisionFrame.radius, ref m_OnGround);
            PhysicsCheck.Coyote(ref m_CoyoteTicks, m_CoyoteBuffer, m_OnGround, Time.deltaTime);
            PhysicsCheck.AntiGravityApex(ref m_AntiGravityTicks, m_AntiGravityBuffer, m_OnGround, m_Rising, Time.deltaTime);
            PhysicsCheck.Rising(body.velocity, ref m_Rising);
            
            // Actions.
            PhysicsAction.Gravity(body, input.HoldJump, state.Weight, state.Sink, m_Rising, m_AntiGravityTicks, m_AntiGravityFactor);
            PhysicsAction.Jump(body, input, input.Jump, state.JumpSpeed, m_OnGround, m_CoyoteTicks);
        }

        // Runs once every fixed interval.
        public void OnFixedUpdate(Rigidbody2D body, Input input, State state, float deltaTime) {
            // Checks.
            // Actions.
            if (m_OnGround) {
                PhysicsAction.Move(body, input.MoveDirection, state.Speed, state.Acceleration, deltaTime);
            }
            else {
                PhysicsAction.Move(body, input.MoveDirection, state.AirSpeed, state.AirAcceleration, deltaTime);
            }
        }

        #endregion

    }

}

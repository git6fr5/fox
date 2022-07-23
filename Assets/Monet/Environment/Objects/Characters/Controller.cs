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

        // Projectile.
        [SerializeField] protected Projectile m_Weapon;
        public Projectile MainWeapon => m_Weapon;
        [SerializeField] private List<string> m_Targets = new List<string>();
        public List<string> Targets => m_Targets;

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

        // Special.
        [SerializeField] private bool m_UnlockedDoubleJump;
        public bool UnlockedDoubleJump => m_UnlockedDoubleJump;
        [SerializeField, ReadOnly] private bool m_DoubleJumpReset;
        public bool DoubleJumpReset => m_DoubleJumpReset;
        [SerializeField] private bool m_UnlockedDash;
        public bool UnlockedDash => m_UnlockedDash;
        [SerializeField, ReadOnly] private bool m_DashReset;
        public bool DashReset => m_DashReset;


        /* --- Initialization --- */
        #region Initialization

        public void Init() {

        }
        

        #endregion

        /* --- Process --- */
        #region Unity 

        public void OnUpdate(Rigidbody2D body, Input input, State state) {        
            // Actions.
            if (m_KnockbackTicks > 0f) { return; }
            PhysicsAction.Jump(body, input, input.Jump, state.JumpSpeed, m_OnGround, ref m_CoyoteTicks);
            PhysicsAction.DoubleJump(body, input, input.Jump, state.DoubleJumpSpeed, m_OnGround, m_CoyoteTicks, ref m_DoubleJumpReset);
            PhysicsAction.Dash(body, input, input.Dash, input.DashDirection, state.DashSpeed, ref m_KnockbackTicks, state.DashDuration, ref m_DashReset);
            if (m_Weapon != null) {
                m_Weapon.Fire(input, input.Attack, input.AttackDirection, m_Targets);
            }
        }

        // Runs once every fixed interval.
        public void OnFixedUpdate(Rigidbody2D body, CircleCollider2D collisionFrame, Input input, State state, float deltaTime) {
            // Checks.
            PhysicsCheck.OnGround(body.position + collisionFrame.offset, collisionFrame.radius, ref m_OnGround);
            PhysicsCheck.Rising(body.velocity, ref m_Rising, m_OnGround);
            PhysicsCheck.Reset(ref m_DoubleJumpReset, m_UnlockedDoubleJump, m_OnGround);
            PhysicsCheck.Reset(ref m_DashReset, m_UnlockedDash, m_OnGround);
            Timer.CountdownTicks(ref m_AntiGravityTicks, m_OnGround || m_Rising, m_AntiGravityBuffer, deltaTime);
            Timer.CountdownTicks(ref m_CoyoteTicks, m_OnGround, m_CoyoteBuffer, deltaTime);
            bool finishKnockback = Timer.UpdateTicks(ref m_KnockbackTicks, false, 0f, deltaTime);
            if (m_Weapon != null) {
                m_Weapon.OnUpdate(deltaTime);
            }
            
            // Actions.
            if (m_KnockbackTicks > 0f) { return; }
            float acceleration = Controller.GetAcceleration(state, m_OnGround);
            float speed = Controller.GetSpeed(state, input.Attack);
            PhysicsAction.Move(body, input.MoveDirection, speed, acceleration, finishKnockback, deltaTime);

            float weight = Controller.GetWeight(state, m_Rising, m_OnGround, m_DoubleJumpReset, m_UnlockedDoubleJump);
            PhysicsAction.Gravity(body, input.HoldJump, weight, state.Sink, m_OnGround, m_Rising, m_AntiGravityTicks, m_AntiGravityFactor);

        }

        public void Knockback(Rigidbody2D body, Vector2 velocity, float duration) {
            body.velocity = velocity;
            body.gravityScale = 0f;
            m_KnockbackTicks = duration;
        }

        public static float GetSpeed(State state, bool attack) {
            if (attack) {
                return 0f;
            }
            return state.Speed;
        }

        public static float GetAcceleration(State state, bool onGround) {
            if (!onGround) {
                return state.AirAcceleration;
            }
            return state.Acceleration;
        }

        public static float GetWeight(State state, bool rising, bool onGround, bool doubleJumpReset, bool doubleJumpUnlocked) {
            if (!doubleJumpReset && doubleJumpUnlocked && rising) {
                return state.DoubleJumpWeight;
            }
            return state.Weight;
        }

        #endregion

    }

}

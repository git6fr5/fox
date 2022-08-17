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

        #region Variables

        // Projectile.
        [SerializeField] protected Weapon m_Weapon;
        public Weapon MainWeapon => m_Weapon;
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

        // Double Jump.
        [SerializeField] private bool m_UnlockedDoubleJump;
        public bool UnlockedDoubleJump => m_UnlockedDoubleJump;
        [SerializeField, ReadOnly] private bool m_DoubleJumpReset;
        public bool DoubleJumpReset => m_DoubleJumpReset;
        [SerializeField, ReadOnly] private int m_DoubleJumpCounter;

        // Dash.
        [SerializeField] private bool m_UnlockedDash;
        public bool UnlockedDash => m_UnlockedDash;
        [SerializeField, ReadOnly] private bool m_DashReset;
        public bool DashReset => m_DashReset;
        [SerializeField, ReadOnly] private float m_DashCooldownTicks;
        public float DashCooldownTicks => m_DashCooldownTicks;
        private static float m_DashCooldown = 0.25f;

        // Slam.
        [SerializeField] private bool m_UnlockedSlam;
        public bool UnlockedSlam => m_UnlockedSlam;
        [SerializeField, ReadOnly] private bool m_SlamReset;
        public bool SlamReset => m_SlamReset;
        [SerializeField, ReadOnly] private bool m_Slamming;

        // Charge Abilities.
        private static float ChargeBuffer = 0.5f;

        // Charge Jump.
        [SerializeField] private bool m_UnlockedChargeJump;
        public bool UnlockedChargeJump => m_UnlockedChargeJump;
        [SerializeField, ReadOnly] private bool m_ChargeJumpReset;
        public bool ChargeJumpReset => m_ChargeJumpReset;
        [SerializeField, ReadOnly] private float m_ChargeJumpTicks;
        [SerializeField, ReadOnly] private bool m_ChargeJumping;
        public float JumpCharge => 1f - m_ChargeJumpTicks / ChargeBuffer;
        public bool ChargingJump => JumpCharge > 0f;

        // Charge Dash.
        [SerializeField] private bool m_UnlockedChargeDash;
        public bool UnlockedChargeDash => m_UnlockedChargeDash;
        [SerializeField, ReadOnly] private bool m_ChargeDashReset;
        public bool ChargeDashReset => m_ChargeDashReset;
        [SerializeField, ReadOnly] private float m_ChargeDashTicks;
        public float DashCharge => 1f - m_ChargeDashTicks / ChargeBuffer;
        public bool ChargingDash => DashCharge > 0f;

        // Flying.
        [SerializeField] private bool m_Flying;

        #endregion

        /* --- Process --- */
        #region Unity 

        public void OnStart(Character character, Rigidbody2D body) {
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (m_Weapon != null) {
                m_Weapon.OnStart(character, m_Targets);
            }
        }

        public void OnStop(Rigidbody2D body) {
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void OnUpdate(Rigidbody2D body, Input input, State state) {        
            if (m_KnockbackTicks > 0f || m_Slamming) { return; }

            // Regular.
            bool charging = input.Block && m_OnGround;
            bool canDash = m_DashCooldownTicks == 0f;
            PhysicsAction.Jump(body, input, input.Jump && !charging, state.JumpSpeed, m_OnGround, ref m_CoyoteTicks);
            PhysicsAction.DoubleJump(body, input, input.Jump && !input.Block, state.DoubleJumpSpeed, m_OnGround, m_CoyoteTicks, ref m_DoubleJumpCounter, ref m_DoubleJumpReset);
            PhysicsAction.Dash(body, input, input.Dash && canDash, input.DashDirection, state.DashSpeed, state.DashDuration, ref m_DashReset, ref m_KnockbackTicks, this);
            
            bool slam = input.Jump && input.Block && !m_OnGround;
            PhysicsAction.Slam(body, input, input.Jump && input.Block && !m_OnGround, ref m_SlamReset, ref m_Slamming);
            
            // Charged
            // Either release one, or release both at the same time.
            // bool releaseChargeJump = ChargingJump && ((!input.HoldJump && input.Block) || (input.HoldJump || input.JumpRelease) && input.BlockRelease);
            bool releaseChargeJump = ChargingJump && (input.JumpRelease && input.BlockRelease);
            PhysicsAction.ChargeJump(body, input, releaseChargeJump, state.ChargeJumpHeight, state.Weight, ref m_ChargeJumpTicks, ChargeBuffer, ref m_ChargeJumpReset, ref m_ChargeJumping);
            // bool releaseChargeDash = ChargingDash && ((!input.HoldDash && input.Block) || (input.HoldDash || input.DashRelease) && input.BlockRelease);
            // bool releaseChargeDash = ChargingDash && (input.DashRelease && input.BlockRelease);
            // PhysicsAction.ChargeDash(body, input, releaseChargeDash, input.FacingDirection, state.DashSpeed, state.ChargeDashDistance, ref m_ChargeDashTicks, ChargeBuffer, ref m_ChargeDashReset, ref m_KnockbackTicks);

            // Attack.
            if (m_Weapon != null) {
                m_Weapon.Attack(input.Attack, input.AttackDirection, m_Targets);
                m_Weapon.Block(input.Block, input.AttackDirection);
            }

        }

        // Runs once every fixed interval.
        public void OnFixedUpdate(Rigidbody2D body, CircleCollider2D collisionFrame, Input input, State state, float deltaTime) {
            // Checks.
            PhysicsCheck.OnGround(body.position + collisionFrame.offset, collisionFrame.radius, ref m_OnGround);
            PhysicsCheck.Rising(body.velocity, ref m_Rising, m_OnGround);
            Timer.CountdownTicks(ref m_DashCooldownTicks, !m_DashReset, m_DashCooldown, deltaTime);
            Timer.StartIf(ref m_KnockbackTicks, 0.01f, m_OnGround && m_Slamming);

            if (!m_OnGround) {
                m_ChargeJumpReset = false;
                m_ChargeDashReset = false;
            }
            bool chargeJump = m_OnGround && (input.HoldJump || input.JumpRelease) && (input.Block || input.BlockRelease);
            bool chargeDash = false; // m_OnGround && (input.HoldDash || input.DashRelease) && (input.Block || input.BlockRelease);

            // Resets.
            PhysicsCheck.Reset(ref m_DoubleJumpReset, m_UnlockedDoubleJump, m_OnGround);
            PhysicsCheck.Reset(ref m_DashReset, m_UnlockedDash, m_OnGround);
            PhysicsCheck.Reset(ref m_SlamReset, m_UnlockedSlam, m_OnGround);
            PhysicsCheck.Reset(ref m_ChargeJumpReset, m_UnlockedChargeJump, m_OnGround && !chargeJump);
            PhysicsCheck.Reset(ref m_ChargeDashReset, m_UnlockedChargeDash, m_OnGround && !chargeDash);
            m_ChargeJumping = m_OnGround || input.Jump ? false : m_ChargeJumping;
            m_Slamming = m_OnGround ? false : m_Slamming;
            m_DoubleJumpCounter = m_OnGround ? state.DoubleJumps : m_DoubleJumpCounter;

            // Buffers.
            Timer.CountdownTicks(ref m_AntiGravityTicks, m_OnGround || m_Rising, m_AntiGravityBuffer, deltaTime);
            Timer.CountdownTicks(ref m_CoyoteTicks, m_OnGround, m_CoyoteBuffer, deltaTime);

            Timer.CountdownTicks(ref m_ChargeJumpTicks, !(m_ChargeJumpReset && chargeJump), ChargeBuffer, deltaTime);
            Timer.CountdownTicks(ref m_ChargeDashTicks, !(m_ChargeDashReset && chargeDash), ChargeBuffer, deltaTime);

            // Weapon Cooldown.
            if (m_Weapon != null) {
                m_Weapon.OnUpdate(deltaTime);
            }

            // Knockback.
            bool finishKnockback = Timer.UpdateTicks(ref m_KnockbackTicks, false, 0f, deltaTime);
            if (m_KnockbackTicks > 0f || m_Slamming) { return; }

            // Movement.
            float acceleration = Controller.GetAcceleration(state, m_OnGround);
            float speed = Controller.GetSpeed(state, input.Attack, input.Block);
            if (m_Flying) {
                PhysicsAction.Move(body, input.FlyDirection, speed, acceleration, finishKnockback, deltaTime, Game.Physics.FlyResistance);
            }
            else {
                PhysicsAction.Move(body, input.MoveDirection, speed, acceleration, finishKnockback, deltaTime);
            }
            float weight = Controller.GetWeight(state, m_Rising, m_OnGround, m_DoubleJumpReset, m_UnlockedDoubleJump, m_Flying);
            PhysicsAction.Gravity(body, input.HoldJump || m_ChargeJumping, weight, state.Sink, m_OnGround, m_Rising, m_AntiGravityTicks, m_AntiGravityFactor);

        }

        public void Knockback(Rigidbody2D body, Vector2 velocity, float duration) {
            if (m_Slamming) { return; }
            body.velocity = velocity;
            body.gravityScale = 0f;
            m_KnockbackTicks = duration;
        }

        public static float GetSpeed(State state, bool attack, bool block) {
            if (block) {
                return state.Speed * 0.25f;
            }
            if (attack) {
                return state.Speed * 0.5f;
            }
            return state.Speed;
        }

        public static float GetAcceleration(State state, bool onGround) {
            if (!onGround) {
                return state.AirAcceleration;
            }
            return state.Acceleration;
        }

        public static float GetWeight(State state, bool rising, bool onGround, bool doubleJumpReset, bool doubleJumpUnlocked, bool flying) {
            if (flying) {
                return 0f;
            }
            if (!doubleJumpReset && doubleJumpUnlocked && rising) {
                return state.DoubleJumpWeight;
            }
            return state.Weight;
        }

        #endregion

    }

}

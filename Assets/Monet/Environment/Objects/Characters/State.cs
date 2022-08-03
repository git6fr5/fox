/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monet {

    ///<summary>
    /// A set of data that defines the state
    /// of a character.
    ///<summary>
    [System.Serializable]
    public class State {

        #region Variables.

        // Health.
        [SerializeField] private int m_MaxHealth;
        public int MaxHealth => m_MaxHealth;
        [SerializeField] private int m_Health;
        public int Health => m_Health;

        // Hurt.
        public bool Immune => m_ImmuneTicks > 0f;
        [SerializeField, ReadOnly] private float m_ImmuneTicks;
        [SerializeField] private float m_ImmuneBuffer = 0.2f; // 0.25f for player.
        public float ImmuneBuffer => m_ImmuneBuffer;
        [SerializeField] private int m_HitStopFrames = 4; // 16 for player.
        public int HitStopFrames => m_HitStopFrames;
        [SerializeField] private float m_ShakeStrength = 0.2f; // 16 for player.
        public float ShakeStrength => m_ShakeStrength;
        [SerializeField] private float m_DeathBuffer = 0.35f; // 0.5f for player.
        public float DeathBuffer => m_DeathBuffer;

        // Speed.
        [SerializeField] protected float m_Speed;
        public float Speed => m_Speed;
        [SerializeField] protected float m_Acceleration;
        public float Acceleration => m_Acceleration;
        [SerializeField] protected float m_AirSpeed;
        public float AirSpeed => m_AirSpeed;
        [SerializeField] protected float m_AirAcceleration;
        public float AirAcceleration => m_AirAcceleration;

        // Jump.
        [SerializeField] private float m_JumpHeight;
        [SerializeField] private float m_RisingTime;
        [SerializeField] private float m_FallingTime;
        [SerializeField, ReadOnly] private float m_JumpSpeed;
        public float JumpSpeed => m_JumpSpeed;
        [SerializeField, ReadOnly] private float m_Weight;
        public float Weight => m_Weight;
        [SerializeField, ReadOnly] private float m_Sink;
        public float Sink => m_Sink;

        // Double Jump
        [SerializeField] private float m_DoubleJumpHeight;
        [SerializeField] private float m_DoubleJumpRisingTime;
        [SerializeField, ReadOnly] private float m_DoubleJumpSpeed;
        public float DoubleJumpSpeed => m_DoubleJumpSpeed;
        [SerializeField, ReadOnly] private float m_DoubleJumpWeight;
        public float DoubleJumpWeight => m_DoubleJumpWeight;
        [SerializeField] private int m_DoubleJumps;
        public int DoubleJumps => m_DoubleJumps;

        // Dash.
        [SerializeField] private float m_DashDistance;
        [SerializeField] private float m_DashDuration;
        public float DashDuration => m_DashDuration;
        [SerializeField, ReadOnly] private float m_DashSpeed;
        public float DashSpeed => m_DashSpeed;

        // Charge Jump.
        [SerializeField] private float m_ChargeJumpHeight;
        public float ChargeJumpHeight => m_ChargeJumpHeight;

        // Charge Dash.
        [SerializeField] private float m_ChargeDashDistance;
        public float ChargeDashDistance => m_ChargeDashDistance;
        
        // Respawn Anchor.
        [SerializeField, ReadOnly] private RespawnStation m_RespawnStation;
        public RespawnStation CurrentRespawnStation => m_RespawnStation;
        public bool CanRespawn => m_RespawnStation != null;

        #endregion

        // Called when this character starts/respawns.
        public void OnStart() {
            m_Health = m_MaxHealth;
            m_ImmuneTicks = 0f;
        }

        // Called when the character this is attached to dies.
        public void OnDeath(Transform transform) {
            if (CanRespawn) {
                Vector3 respawnPosition = m_RespawnStation.transform.position;
                transform.position = respawnPosition + Vector3.up * 2f;
            }
        }

        // Runs once every frame in order to calculate the settings.
        public void OnUpdate() {
            PhysicsSettings.CalculateJump(m_JumpHeight, m_RisingTime, m_FallingTime, ref m_JumpSpeed, ref m_Weight, ref m_Sink);
            PhysicsSettings.CalculateJump(m_DoubleJumpHeight, m_DoubleJumpRisingTime, ref m_DoubleJumpSpeed, ref m_DoubleJumpWeight);
            PhysicsSettings.CalculateDash(m_DashDistance, m_DashDuration, ref m_DashSpeed);
        }

        // Called once every fixed interval in order to increment timers.
        public void OnFixedUpdate(float deltaTime) {
            Timer.TickDown(ref m_ImmuneTicks, deltaTime);
        }

        // Hurt the character that this state is attached to.
        public void OnHurt(int value, bool feedback = true) {
            // Provide visual feedback.
            if (feedback) {
                Screen.Shake(m_ShakeStrength, m_ImmuneBuffer);
                Game.HitStop(m_HitStopFrames);
            }

            // Increment the values.
            m_Health -= value;
            m_Health = m_Health <= 0 ? 0 : m_Health;

            // Provide some immunity frames.
            Timer.Start(ref m_ImmuneTicks, m_ImmuneBuffer);
        }

        // heal the character that this state is attached to.
        public void OnHeal(int value = -1) {
            // Provide a full heal by default.
            if (value == -1) {
                value = m_MaxHealth;
                return;
            }

            // Increment the values.
            m_Health += value;
            m_Health = m_Health > m_MaxHealth ? m_MaxHealth : m_Health;

        }

        public void SetRespawn(RespawnStation respawnStation) {
            m_RespawnStation = respawnStation;
        }

    }

}


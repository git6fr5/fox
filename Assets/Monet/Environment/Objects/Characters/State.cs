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

        // Health.
        [SerializeField] private int m_MaxHealth;
        public int MaxHealth => m_MaxHealth;
        [SerializeField] private int m_Health;
        public int Health => m_Health;

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

        // Dash.
        [SerializeField] private float m_DashDistance;
        [SerializeField] private float m_DashTime;
        public float DashTime => m_DashTime;
        [SerializeField, ReadOnly] private float m_DashSpeed;
        public float DashSpeed => m_DashSpeed;

        // Respawn Anchor.
        [SerializeField, ReadOnly] private RespawnStation m_RespawnStation;

        public void Init() {
            m_Health = m_MaxHealth;
        }

        public void OnUpdate() {
            PhysicsSettings.CalculateJump(m_JumpHeight, m_RisingTime, m_FallingTime, ref m_JumpSpeed, ref m_Weight, ref m_Sink);
            PhysicsSettings.CalculateJump(m_DoubleJumpHeight, m_DoubleJumpRisingTime, ref m_DoubleJumpSpeed, ref m_DoubleJumpWeight);
            PhysicsSettings.CalculateDash(m_DashDistance, m_DashTime, ref m_DashSpeed);
        }

        public void Hurt(int value) {
            m_Health -= value;
            if (m_Health <= 0) {
                m_Health = 0;
            }
        }

        public void Heal(int value = -1) {
            if (value == -1) {
                value = m_MaxHealth;
            }
            m_Health += value;
            if (m_Health > m_MaxHealth) {
                m_Health = m_MaxHealth;
            }
        }

        public void SetRespawn(RespawnStation respawnStation) {
            m_RespawnStation = respawnStation;
        }

    }

}


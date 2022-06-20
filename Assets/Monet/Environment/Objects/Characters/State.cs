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

        // Respawn Anchor.
        [SerializeField, ReadOnly] private RespawnStation m_RespawnStation;

        public void Init() {
            m_Health = m_MaxHealth;
        }

        public void OnUpdate() {
            PhysicsSettings.CalculateJump(m_JumpHeight, m_RisingTime, m_FallingTime, ref m_JumpSpeed, ref m_Weight, ref m_Sink);
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


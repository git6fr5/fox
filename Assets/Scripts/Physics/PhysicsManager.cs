/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// Defines the general physics settings for the game.
    ///<summary>
    public class PhysicsManager : MonoBehaviour {

        // Movement.
        [SerializeField] 
        private float m_MovementPrecision = 1f/32f;
        public float MovementPrecision => m_MovementPrecision;

        // Collision.
        [SerializeField] 
        private float m_CollisionPrecision = 1f/48f;
        public float CollisionPrecision => m_CollisionPrecision;

        // Gravity
        [SerializeField] 
        private float m_GravityScale = 1f;
        public float GravityScale => m_GravityScale;

        // Collision.
        [SerializeField] 
        private CollisionLayers m_CollisionLayers;
        public CollisionLayers CollisionLayers => m_CollisionLayers;

        // Ramp stop.
        [SerializeField, ReadOnly] 
        private bool m_RampStop = false;

        [SerializeField, ReadOnly]
        private float m_RampIncrement = 1f / 128f;

        [SerializeField, ReadOnly]
        private float m_Ramp = 0f;

        // Hit stop.
        [SerializeField, ReadOnly]
        private bool m_HitStop;

        [SerializeField, ReadOnly]
        private int m_HitFrames = 0;
        
        [SerializeField, ReadOnly]
        private int m_StopFrames = 16;

        // Ticks.
        [SerializeField, ReadOnly] 
        private float m_TimeScale = 1;
        public float TimeScale => m_TimeScale;

        [SerializeField, ReadOnly]
        private float m_Ticks;
        public float Ticks => m_Ticks;

        // Pause the game.
        public void Pause() {
            m_TimeScale = 0f;
        }

        // Runs once every frame.
        void Update() {
            Time.timeScale = m_TimeScale;
            UpdateRamp();
            UpdateHit();
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
        }

        // Run a hit stop.
        public void HitStop(int frames = 16) {
            if (m_RampStop) { return; }
            // Pause the game.
            Pause();
            // Set up the hitstop
            m_HitStop = true;
            m_HitFrames = 0;
            m_StopFrames = frames;
        }

        private void UpdateHit() {
            if (m_HitStop) {
                m_HitFrames += 1;
                if (m_HitFrames >= m_StopFrames) {
                    m_TimeScale = 1f;
                }
            }
        }

        // Run a ramp stop.
        public void RampStop(int frames = 128) {
            // Pause the game.
            Pause();
            // Set up the rampstop
            m_Ramp = 0f;
            m_RampStop = true;
            m_RampIncrement = 1f / (float)frames;
            // Disable the hitstop.
            m_HitStop = false;
            m_HitFrames = 0;
        }

        private void UpdateRamp() {
            if (m_RampStop) {
                m_Ramp += m_RampIncrement;
                if (m_Ramp > 0.5f) {
                    m_TimeScale += m_RampIncrement;
                }
                if (m_TimeScale >= 1f) {
                    m_RampStop = false;
                    m_TimeScale = 1f;
                }
            }
        }

    }
}
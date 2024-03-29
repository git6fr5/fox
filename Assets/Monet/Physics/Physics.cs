/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// Stores a set of collision layers for easy reference.
    ///<summary>
    [System.Serializable]
    public struct CollisionLayer {

        // Collision Layers
        [SerializeField] private LayerMask m_Ground;
        public LayerMask Ground => m_Ground;
        [SerializeField] private LayerMask m_Water;
        public LayerMask Water => m_Water;
        [SerializeField] private LayerMask m_Platform;
        public LayerMask Platform => m_Platform;
        [SerializeField] private LayerMask m_Characters;
        public LayerMask Characters => m_Characters;
        [SerializeField] private LayerMask m_Projectiles;
        public LayerMask Projectiles => m_Projectiles;
        [SerializeField] private LayerMask m_Opaque;
        public LayerMask Opaque => m_Opaque;

    }

    ///<summary>
    /// Does the physics for the game.
    ///<summary>
    [System.Serializable]
    public class PhysicsSettings {

        // Movement.
        [SerializeField] private float m_MovementPrecision = 0.05f;
        public float MovementPrecision => m_MovementPrecision;

        // Collision.
        [SerializeField] private float m_CollisionPrecision = 0.045f;
        public float CollisionPrecision => m_CollisionPrecision;

        // Gravity
        [SerializeField] private float m_GravityScale = 1f;
        public float GravityScale => m_GravityScale;

        // Collision.
        [SerializeField] private float m_FlyResistance = 0.975f;
        public float FlyResistance => m_FlyResistance;

        // Collision.
        [SerializeField] private CollisionLayer m_CollisionLayers;
        public CollisionLayer CollisionLayers => m_CollisionLayers;

        // Calculations.
        public static void CalculateJump(float height, float risingTime, ref float speed, ref float weight) {
            speed = 2f * height / risingTime;
            weight = 2f * height / (risingTime * risingTime) / Mathf.Abs(UnityEngine.Physics2D.gravity.y * Game.Physics.GravityScale);
        }

        // Calculations.
        public static float GetJumpSpeed(float height, float weight) {
            float factor = Mathf.Abs(UnityEngine.Physics2D.gravity.y * Game.Physics.GravityScale);
            return 2f * height / Mathf.Sqrt(2f * height / (weight * factor));
        }

        public static float GetDashDuration(float distance, float speed) {
            return distance / speed;
        }

        public static void CalculateJump(float height, float risingTime, float fallingTime, ref float speed, ref float weight, ref float floatiness) {
            speed = 2f * height / risingTime;
            weight = 2f * height / (risingTime * risingTime) / Mathf.Abs(UnityEngine.Physics2D.gravity.y * Game.Physics.GravityScale);
            floatiness = (fallingTime * fallingTime) * weight * Mathf.Abs(UnityEngine.Physics2D.gravity.y * Game.Physics.GravityScale) / (2f * height);
            floatiness = 1f / floatiness;
        }

        public static void CalculateDash(float distance, float time, ref float speed) {
            speed = distance / time;
        }

    }

}



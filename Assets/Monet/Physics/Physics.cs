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

    }

    ///<summary>
    /// Does the physics for the game.
    ///<summary>
    [System.Serializable]
    public class PhysicsSettings {

        // Movement.
        [SerializeField] private float m_MovementPrecision = 0.05f;
        public float MovementPrecision => m_MovementPrecision;

        // Gravity
        [SerializeField] private float m_GravityScale = 1f;
        public float GravityScale => m_GravityScale;

        // Collision.
        [SerializeField] private CollisionLayer m_CollisionLayers;
        public CollisionLayer CollisionLayers => m_CollisionLayers;

        // Calculations.
        public static void CalculateJump(float height, float risingTime, float fallingTime, ref float speed, ref float weight, ref float floatiness) {
            speed = 2f * height / risingTime;
            weight = 2f * height / (risingTime * risingTime) / Mathf.Abs(UnityEngine.Physics2D.gravity.y * Game.Physics.GravityScale);
            floatiness = (fallingTime * fallingTime) * weight * Mathf.Abs(UnityEngine.Physics2D.gravity.y * Game.Physics.GravityScale) / (2f * height);
            floatiness = 1f / floatiness;
        }

    }

}



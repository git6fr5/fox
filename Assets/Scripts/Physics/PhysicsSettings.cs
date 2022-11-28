/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// Defines the general physics settings for the game.
    ///<summary>
    public class PhysicsSettings : MonoBehaviour {

        // Movement.
        [SerializeField] private float m_MovementPrecision = 1f/32f;
        public float MovementPrecision => m_MovementPrecision;

        // Collision.
        [SerializeField] private float m_CollisionPrecision = 1f/48f;
        public float CollisionPrecision => m_CollisionPrecision;

        // Gravity
        [SerializeField] private float m_GravityScale = 1f;
        public float GravityScale => m_GravityScale;

        // Collision.
        [SerializeField] private CollisionLayers m_CollisionLayers;
        public CollisionLayers CollisionLayers => m_CollisionLayers;

    }
}
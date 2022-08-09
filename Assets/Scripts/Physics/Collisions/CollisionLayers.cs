/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// Stores a set of collision layers for easy reference.
    ///<summary>
    [System.Serializable]
    public class CollisionLayers {

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
}
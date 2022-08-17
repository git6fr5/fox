/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer;
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// Defines useful and easy to use extensions for the rigidbody.
    ///<summary>
    public static class Rigidbody2DExtensions {

        // Moves the rigidbodies position.
        public static void Move(this Rigidbody2D rb, Vector2 dx) {
            rb.position += dx;
        }

        // Adds to the rigidbodies velocity.
        public static void AddVelocity(this Rigidbody2D rb, Vector2 dv) {
            rb.velocity += dv;
        }

        // Sets the rigidbodies velocity.
        public static void SetVelocity(this Rigidbody2D rb, Vector2 v) {
            rb.velocity = v;
        }

        // Slows this body by the given factor.
        public static void Slowdown(this Rigidbody rb, float c) {
            rb.velocity *= c;
        }

        // Clamps the rigidbodies fall speed.
        public static void ClampFallSpeed(this Rigidbody2D rb, float s) {
            rb.velocity = rb.velocity.y < -s ? new Vector2(rb.velocity.x, -s) : rb.velocity;
        }

        // Clamps the rigidbodies gravity scale.
        public static void SetWeight(this Rigidbody2D rb, float w) {
            rb.gravityScale = Game.Physics.GravityScale * w;
        }

        // Checks if this body is rising.
        public static bool Rising(this Rigidbody2D rb) {
            return rb.velocity.y > 0f;
        }

    }
    
}
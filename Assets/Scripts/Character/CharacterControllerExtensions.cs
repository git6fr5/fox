/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Character;

/* --- Defintions --- */
using Game = Platformer.GameManager;

namespace Platformer.Character {

    ///<summary>
    /// Defines useful and easy to use extensions for the character controller..
    ///<summary>
    public static class CharacterControllerExtensions {

        // Moves the rigidbodies position.
        public static void Move(this CharacterController controller, Vector2 dx) {
            controller.CharacterBody.position += dx;
        }

        // Adds to the rigidbodies velocity.
        public static void AddVelocity(this CharacterController controller, Vector2 dv) {
            controller.CharacterBody.velocity += dv;
        }

        // Sets the rigidbodies velocity.
        public static void SetVelocity(this CharacterController controller, Vector2 v) {
            controller.CharacterBody.velocity = v;
        }

        // Slows this body by the given factor.
        public static void Slowdown(this CharacterController controller, float c) {
            controller.CharacterBody.velocity *= c;
        }

        // Clamps the rigidbodies fall speed.
        public static void ClampFallSpeed(this CharacterController controller, float s) {
            controller.CharacterBody.velocity = controller.CharacterBody.velocity.y < -s ? new Vector2(controller.CharacterBody.velocity.x, -s) : controller.CharacterBody.velocity;
        }

        // Clamps the rigidbodies rising speed.
        public static void ClampRiseSpeed(this CharacterController controller, float s) {
            controller.CharacterBody.velocity = controller.CharacterBody.velocity.y > s ? new Vector2(controller.CharacterBody.velocity.x, s) : controller.CharacterBody.velocity;
        }

        // Clamps the rigidbodies gravity scale.
        public static void SetWeight(this CharacterController controller, float w) {
            controller.CharacterBody.gravityScale = Game.Physics.GravityScale * w;
        }

    }
    
}
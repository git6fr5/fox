/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// A set of functions that define simple physical actions.
    ///<summary>
    public class PhysicsAction {

        public static void Move(Rigidbody2D body, float direction, float speed, float acceleration, float deltaTime) {

            // Cache the target and current velocities.
            float targetSpeed = direction * speed;
            float currentSpeed = body.velocity.x;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * acceleration * deltaTime;

            // Calculate the precision of the change.
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                body.velocity = new Vector2(targetSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(currentSpeed + deltaSpeed, body.velocity.y);
            }

        }

        public static void Jump(Rigidbody2D body, Input input, bool jump, float jumpSpeed, bool onGround, float coyote) {
            if (jump && (onGround || coyote > 0f)) {
                body.velocity = Vector2.up * jumpSpeed;
                input.ResetJump(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void Gravity(Rigidbody2D body, bool hold, float weight, float sink, bool rising, float antiGravTicks, float antiGravFactor) {
            body.gravityScale = Game.Physics.GravityScale;
            if (hold && rising) {
                body.gravityScale *= weight;
            }
            else {
                body.gravityScale *= (weight * sink);
                if (!rising && antiGravTicks > 0f) {
                    body.gravityScale *= antiGravFactor;
                }
            }
        }


    }

}
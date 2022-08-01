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

        public static void Move(Rigidbody2D body, float direction, float speed, float acceleration, bool finishKnockback, float deltaTime) {

            // Hard decelleration at the end of knockback.
            if (finishKnockback) {
                body.velocity *= 0.25f;
            }

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

        public static void Move(Rigidbody2D body, Vector2 direction, float speed, float acceleration, bool finishKnockback, float deltaTime, float resistance) {

            // Hard decelleration at the end of knockback.
            if (finishKnockback) {
                body.velocity *= 0.25f;
            }

            Vector2 targetVelocity = direction.normalized * speed;
            Vector2 deltaVelocity = (targetVelocity - body.velocity).normalized * acceleration * deltaTime;
            body.velocity += deltaVelocity;

            if (body.velocity.magnitude > speed) {
                body.velocity = speed * body.velocity.normalized;
            }

            // Resistance
            if (targetVelocity == Vector2.zero) {
                body.velocity *= resistance;
            }
            // Check for released inputs.
            if (targetVelocity.y == 0f && Mathf.Abs(body.velocity.y) < Mathf.Abs(deltaVelocity.y)) {
                body.velocity = new Vector2(body.velocity.x, 0f);
            }
            if (targetVelocity.x == 0f && Mathf.Abs(body.velocity.x) < Mathf.Abs(deltaVelocity.x)) {
                body.velocity = new Vector2(0f, body.velocity.y);
            }

        }

        public static void Jump(Rigidbody2D body, Input input, bool jump, float jumpSpeed, bool onGround, ref float coyote) {
            if (jump && (onGround || coyote > 0f)) {
                body.position += Vector2.up * Game.Physics.MovementPrecision;
                if (onGround || body.velocity.y < 0f) {
                    body.velocity = Vector2.up * jumpSpeed;
                }
                else {
                    body.velocity += Vector2.up * jumpSpeed;
                }
                coyote = 0f;
                input.ResetJump(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void DoubleJump(Rigidbody2D body, Input input, bool jump, float jumpSpeed, bool onGround, float coyote, ref bool reset) {
            if (jump && reset && !onGround && coyote <= 0f) {
                body.velocity = Vector2.up * jumpSpeed;
                reset = false;
                input.ResetJump(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void Dash(Rigidbody2D body, Input input, bool dash, Vector2 direction, float dashSpeed, ref float ticks, float duration,ref bool reset) {
            if (dash && reset) {
                body.gravityScale = 0f;
                body.velocity = direction.normalized * dashSpeed;
                reset = false;
                ticks = duration;
                input.ResetDash(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void Gravity(Rigidbody2D body, bool hold, float weight, float sink, bool onGround, bool rising, float antiGravTicks, float antiGravFactor) {
            body.gravityScale = Game.Physics.GravityScale;
            if (onGround) {
                return;
            }

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
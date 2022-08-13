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

        public static void DoubleJump(Rigidbody2D body, Input input, bool jump, float jumpSpeed, bool onGround, float coyote, ref int count, ref bool reset) {
            if (jump && reset && !onGround && coyote <= 0f) {
                body.velocity = Vector2.up * jumpSpeed;
                count -= 1;
                if (count <= 0) {
                    reset = false;
                }
                input.ResetJump(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void ChargeJump(Rigidbody2D body, Input input, bool release, float height, float weight, ref float ticks, float buffer, ref bool reset, ref bool doing) {
            if (release && reset) {
                float charge = 1f - ticks / buffer;
                float speed = PhysicsSettings.GetJumpSpeed((charge * 0.75f + 0.25f) * height, weight);
                body.position += Vector2.up * Game.Physics.MovementPrecision;
                body.velocity = speed * Vector2.up;
                doing = true;
                //
                ticks = buffer;
                reset = false;
                input.ResetJumpRelease(); // This line and requiring the whole input to be passed is ugly.
                input.ResetBlockRelease(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void Dash(Rigidbody2D body, Input input, bool dash, Vector2 direction, float speed, float duration, ref bool reset, ref float knockbackTicks, Controller controller) {
            if (dash && reset) {
                body.gravityScale = 0f;
                body.velocity = Vector2.zero; // direction.normalized * speed;
                float delay = duration / 3f;
                Game.Instance.DelayedDashEffect(body, input, speed, duration, delay, controller);
                knockbackTicks = duration + delay;
                reset = false;
                input.ResetDash(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void ChargeDash(Rigidbody2D body, Input input, bool release, float direction, float speed, float distance, ref float ticks, float buffer, ref bool reset, ref float knockbackTicks) {
            if (release && reset) {
                float charge = 1f - ticks / buffer;
                float duration = PhysicsSettings.GetDashDuration((charge * 0.75f  + 0.25f) * distance, speed);
                body.gravityScale = 0f;
                body.velocity = new Vector2(direction, 0f) * speed;
                knockbackTicks = duration;
                //
                ticks = buffer;
                reset = false;
                input.ResetDashRelease(); // This line and requiring the whole input to be passed is ugly.
                input.ResetBlockRelease(); // This line and requiring the whole input to be passed is ugly.
            }
        }

        public static void Slam(Rigidbody2D body, Input input, bool slam, ref bool reset, ref bool doing) {
            if (slam && reset) {
                body.gravityScale = 0f;
                body.velocity = Vector2.down * 50f;
                reset = false;
                doing = true;
                input.ResetJump();
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
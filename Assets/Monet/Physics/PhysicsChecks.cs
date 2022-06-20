/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// A set of functions that define simple physical checks.
    ///<summary>
    public class PhysicsCheck {

        public static void OnGround(Vector3 center, float radius, ref bool onGround) {
            onGround = false;
            for (int i = -1; i <= 1; i++) {
                Vector3 offset = Vector3.down * radius + i * Vector3.left * radius / 1.5f;
                Collider2D temp = Physics2D.OverlapCircle(center + offset, Game.Physics.MovementPrecision, Game.Physics.CollisionLayers.Ground);
                if (temp != null) {
                    onGround = true;
                    return;
                }
            }
        }

        public static void Coyote(ref float ticks, float buffer, bool onGround, float dt) {
            if (onGround) {
                ticks = buffer;
            }
            else {
                ticks -= dt;
                if (ticks < 0f) {
                    ticks = 0f;
                }
            }
        }

        
        public static void AntiGravityApex(ref float ticks, float buffer, bool onGround, bool rising, float dt) {
            if (onGround) {
                ticks = buffer;
            }
            else {
                if (!rising) {
                    ticks -= dt;
                    if (ticks < 0f) {
                        ticks = 0f;
                    }
                }
            }

        }

        public static void Rising(Vector2 velocity, ref bool rising) {
            rising = velocity.y > 0;
        }

    }

}
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
                Collider2D temp = Physics2D.OverlapCircle(center + offset, Game.Physics.CollisionPrecision, Game.Physics.CollisionLayers.Ground);
                if (temp != null) {
                    onGround = true;
                    return;
                }
            }
        }

        public static void Reset(ref bool reset, bool unlocked, bool condition) {
            if (!unlocked) { 
                reset = false; 
            }
            else {
                reset = condition ? true : reset;
            }
        }

        public static void Rising(Vector2 velocity, ref bool rising, bool onGround) {
            rising = velocity.y > 0f && !onGround;
        }

        public static TMonoBehaviour Closest<TMonoBehaviour>(Vector3 position, float radius, LayerMask layers) where TMonoBehaviour : MonoBehaviour {
            float minDistance = Mathf.Infinity;
            TMonoBehaviour closest = null;
            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(position, radius, layers);
            for (int i = 0; i < colliders.Length; i++) {
                TMonoBehaviour temp = colliders[i].GetComponent<TMonoBehaviour>();
                if (temp != null) {
                    float distance = (temp.transform.position - position).magnitude;
                    if (distance < minDistance) {
                        closest = temp;
                    }
                }
            }
            return closest;
        }

    }

}
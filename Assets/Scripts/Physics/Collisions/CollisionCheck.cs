/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Physics;

namespace Platformer.Physics {

    ///<summary>
    /// A set of functions that define simple collision checks.
    ///<summary>
    public class CollisionCheck {

        // Checks whether anything within the circle is touching something on the given layer.
        public static bool Touching(Vector3 center, float radius, LayerMask layer) {
            for (int i = -1; i <= 1; i++) {
                Vector3 offset = Vector3.down * radius + i * Vector3.left * radius / 1.5f;
                Collider2D temp = Physics2D.OverlapCircle(center + offset, Game.Physics.CollisionPrecision, layer);
                if (temp != null) {
                    return true;
                }
            }
            return false;
        }

        // Finds the closest object of the given class within a specified radius (null if none exist).
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

        // Finds all object of the given class within a specified radius (null if none exist).
        public static List<TMonoBehaviour> All<TMonoBehaviour>(Vector3 position, float radius, LayerMask layers) where TMonoBehaviour : MonoBehaviour {
            List<TMonoBehaviour> list = new List<TMonoBehaviour>();
            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(position, radius, layers);
            for (int i = 0; i < colliders.Length; i++) {
                TMonoBehaviour behaviour = colliders[i].GetComponent<TMonoBehaviour>();
                if (behaviour != null) {
                    list.Add(behaviour);
                }
            }
            return list;
        }

        // Finds all object of the given class interseting the line (null if none exist).
        public static TMonoBehaviour LineOfSight<TMonoBehaviour>(Vector3 position, Vector2 direction, LayerMask layers) where TMonoBehaviour : MonoBehaviour {
            List<TMonoBehaviour> list = new List<TMonoBehaviour>();
            RaycastHit2D hit = UnityEngine.Physics2D.Raycast(position, direction, Mathf.Infinity, layers);
            TMonoBehaviour behaviour = hit.collider.GetComponent<TMonoBehaviour>();
            if (behaviour != null) {
                return behaviour;
            }
            return null;
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Platformer;
using Platformer.Character;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public static class Obstacle {

        // Moves an obstacle towards a target.
        public static void Move(Transform transform, Vector3 destination, float speed, float deltaTime, List<Transform> transforms = null) {
            if (destination == transform.position) {
                return;
            }

            Vector3 displacement = destination - transform.position;
            Vector3 deltaPosition = displacement.normalized * speed * deltaTime;
            if (displacement.magnitude < deltaPosition.magnitude) {
                deltaPosition = displacement;
            }

            transform.position += deltaPosition;
            if (transforms != null) {
                Drag(transforms, deltaPosition);
            }
            
        }

        // Drags a collection of transforms with the obstacle.
        public static void Drag(List<Transform> transforms, Vector3 deltaPosition) {
            for (int i = 0; i < transforms.Count; i++) {
                transforms[i].position += deltaPosition;
            }
        }

        // Cycles an object around a given origin.
        public static void Cycle(Transform transform, float ticks, float period, Vector3 origin, Vector2 ellipse) {
            Vector3 horizontal = Vector3.right * ellipse.x * Mathf.Cos(2f * Mathf.PI * ticks / period);
            Vector3 vertical = Vector3.up * ellipse.y * Mathf.Sin(2f * Mathf.PI * ticks / period);
            transform.position = origin + horizontal + vertical;
        }

        // Shakes the obstacle a little bit.
        public static void Shake(Transform transform, Vector3 origin, float strength) {
            transform.position = origin;
            transform.position += (Vector3)Random.insideUnitCircle * strength;
        }

        // Shakes the obstacle along the horizontal axis.
        public static void HorizontalShake(Transform transform, Vector3 origin, float strength) {
            transform.position = origin;
            transform.position += (Vector3)Random.insideUnitCircle * strength;
            transform.position = new Vector3(transform.position.x, origin.y, origin.z);
        }

        // When an obstacle collides with something.
        public static void OnCollision(Collision2D collision, ref List<Transform> container, bool enter) {
            // Check if there is a character.
            CharacterState character = collision.gameObject.GetComponent<CharacterState>();
            if (character == null) { 
                return; 
            }

            // Edit the collision container as appropriate.
            Transform transform = character.transform; 
            if (enter && !container.Contains(transform)) {
                container.Add(transform);
            }
            else if (!enter && container.Contains(transform)) {
                container.Remove(transform);
            }

        }

        // Check if a character is standing on top of this.
        public static void PressedDown(Vector3 center, List<Transform> container, ref bool pressedDown) {
            pressedDown = false;
            for (int i = 0; i < container.Count; i++) {
                CharacterState character = container[i].GetComponent<CharacterState>();
                if (character != null) {
                    // Check the the characters is in contact and above the obstacle.
                    Vector3 offset = (Vector3)character.Collider.offset; 
                    Vector3 height = Vector3.down * character.Collider.radius;
                    Vector3 position = container[i].position + offset + height;
                    bool standingStill = Mathf.Abs(character.Body.velocity.y) < Game.Physics.MovementPrecision;
                    if (position.y - center.y > 0f && standingStill) {
                        pressedDown = true;
                        return;
                    }
                }
            }
        }

        // Edits the spline of an obstacle
        public static void EditSpline(Spline spline, float length) {
            spline.Clear();
            spline.InsertPointAt(0, new Vector3(-0.5f, 0f, 0f));
            spline.InsertPointAt(1, length * Vector3.right + new Vector3(-0.5f, 0f, 0f));
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        // Edits the hitbox of an obstacle
        public static void EditHitbox(BoxCollider2D box, float length, float height) {
            box.size = new Vector2(length, height);
            box.offset = new Vector2(length - 1f, 1f - height) / 2f;
        }

    }
}
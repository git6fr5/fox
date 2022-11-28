/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Decor;

namespace Platformer.Decor {

    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class Rope : MonoBehaviour {

        /* --- Components --- */
        protected LineRenderer lineRenderer;
        protected EdgeCollider2D edgeCollider;

        /* --- Static Variables --- */
        [SerializeField] protected static float SegmentLength  = 3f/16f;

        /* --- Variables --- */
        [HideInInspector] protected int segmentCount; // The number of segments.
        [SerializeField] public Transform startpoint; // The width of the rope.
        [SerializeField] protected float weight = 1.5f;
        [SerializeField] protected int stiffness = 5;
        [SerializeField] public float ropeLength; // The width of the rope.
        [SerializeField] public float ropeWidth; // The width of the rope.
        [SerializeField] protected Vector3[] ropeSegments; // The current positions of the segments.
        [SerializeField] protected Vector3[] prevRopeSegments; // The previous positions of the segments.
        [SerializeField] protected Vector3[] velocities; // The previous positions of the segments.

        /* --- Unity --- */
        // Runs once on initialization.
        void Awake() {
            // Cache these references.
            lineRenderer = GetComponent<LineRenderer>();
            edgeCollider = GetComponent<EdgeCollider2D>();
            // Set up these components.
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;
            edgeCollider.edgeRadius = ropeWidth;
            OnAwake();
            RopeSegments();
        }

        // Runs once every set time interval.
        void FixedUpdate() {
            Simulation();
        }

        // Runs if this trigger is activated.
        void OnTriggerStay2D(Collider2D collider) {
            if (collider.GetComponent<Rigidbody2D>()) {
                Jiggle(collider);
            }
        }

        /* --- Methods --- */
        // Initalizes the rope segments.
        void RopeSegments() {
            // Get the number of segments for a rope of this length.
            segmentCount = (int)Mathf.Ceil(ropeLength / SegmentLength);

            // Initialize the rope segments.
            ropeSegments = new Vector3[segmentCount];
            prevRopeSegments = new Vector3[segmentCount];
            velocities = new Vector3[segmentCount];
            
            ropeSegments[0] = Vector3.zero;
            prevRopeSegments[0] = ropeSegments[0];
            velocities[0] = Vector2.zero;

            for (int i = 1; i < segmentCount; i++) {
                Vector2 offset = SegmentLength * Random.insideUnitCircle.normalized;
                offset.y = -Mathf.Abs(offset.y);
                ropeSegments[i] = ropeSegments[i - 1] + (Vector3)offset;
                prevRopeSegments[i] = ropeSegments[i];
                velocities[i] = new Vector2(0f, 0f);
            }
        }

        // Adds a jiggle whenever a body collides with this.
        void Jiggle(Collider2D collider) {
            Rigidbody2D body = collider.GetComponent<Rigidbody2D>();
            // Get the segment closest to the collider.
            Vector3 pos = collider.transform.position;
            int index = 1;
            float minDist = 1e9f;
            for (int i = 1; i < segmentCount; i++) {

                Vector3 segPos = transform.position + ropeSegments[i];

                float dist = (pos - segPos).magnitude;
                if (dist < minDist) {
                    index = i;
                    minDist = dist;
                }

            }
            // Add a jiggle to this segment.
            velocities[index] = body.velocity; // body.gravityScale /  
        }

        void Simulation() {
            Vector3 forceGravity = new Vector3(0f, -weight * Game.Physics.GravityScale, 0f);
            for (int i = 0; i < segmentCount; i++) {
                Vector3 velocity = ropeSegments[i] - prevRopeSegments[i];
                prevRopeSegments[i] = ropeSegments[i];
                ropeSegments[i] += velocity * 0.975f;
                ropeSegments[i] += forceGravity * Time.fixedDeltaTime;
                ropeSegments[i] += velocities[i] * Time.fixedDeltaTime;
            }

            for (int i = 0; i < velocities.Length; i++) {
                velocities[i] *= 0.65f;
            }

            for (int i = 0; i < stiffness; i++) {
                Constraints();
            }

            lineRenderer.positionCount = segmentCount;
            lineRenderer.SetPositions(ropeSegments);

            Vector2[] points = new Vector2[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                points[i] = (Vector2)ropeSegments[i];
            }

            edgeCollider.points = points;
            
        }

        protected virtual void OnAwake() {

        }

        protected virtual void Constraints() {
            //
        }

    }

}
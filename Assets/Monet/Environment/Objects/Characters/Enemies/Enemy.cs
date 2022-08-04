/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///  Captures all the generic logic for an enemy.
    ///<summary>
    public abstract class Enemy : Input {

        #region Variables

        // Components.
        public Rigidbody2D Body => GetComponent<Rigidbody2D>();

        // Patrol path.
        [HideInInspector] protected Vector3 m_Origin;
        [SerializeField] protected Vector3[] m_Path = null;
        [SerializeField, ReadOnly] protected int m_PathIndex;

        // Waiting.
        public bool Waiting => m_WaitTicks != 0f;
        public float m_WaitBuffer = 1f;
        [SerializeField] private float m_WaitTicks;
        
        #endregion

        #region Unity

        // Interface that allows the path to be set up from LDtk.
        public void Init(Vector3[] path) {
            m_Origin = transform.position;
            m_Path = path;
        }

        // Runs once on the first frame.
        void Start() {
            Timer.Start(ref m_WaitTicks, m_WaitBuffer);
        }

        // Runs once very fixed interval.
        protected virtual void FixedUpdate() {
            // Calculate these values.
            float distance = Mathf.Abs(m_Path[m_PathIndex].x - transform.position.x);
            float dx = Mathf.Abs(Body.velocity.x) * Time.fixedDeltaTime;
            bool finished = Timer.TickDownIf(ref m_WaitTicks, Time.fixedDeltaTime, Waiting);

            // Start the timer if the close enough to the target.
            Timer.StartIf(ref m_WaitTicks, m_WaitBuffer, distance < dx && !Waiting);
            // Cycle the patrol array index if the timer has finished ticking down.
            Utilities.CycleIndexIf(ref m_PathIndex, 1, m_Path.Length, finished);
        }

        // Search for the player within a certain radius.
        private Player Search(Vector3 origin, Vector3 target, float maxDistance) {
            float distance = (target - origin).magnitude;
            if (distance > maxDistance) {
                return null;
            }

            // Set up the ray.
            Vector3 direction = (target - origin).normalized;
            origin += direction * 0.5f;
            distance -= 0.5f;

            // Cast the ray.
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, Game.Physics.CollisionLayers.Opaque);
            for (int i = 0; i < hits.Length; i++) {
                RaycastHit2D hit = hits[i];
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    return null;
                }
            }
            
            return Game.MainPlayer;
        }

        #endregion

        #region Actions

        // Patrols between two points.
        protected void PatrolAction() {
            if (Waiting) {
                WaitAction();
            }
            else {
                PathAction();
            }
        }

        // Attacks.
        protected void AttackAction() {
            m_Direction = Vector2.zero;
            m_Attack = true;
            m_Jump = false;
            m_HoldJump = false;
        }

        // Waits.
        protected void WaitAction() {
            m_Direction = Vector2.zero;
            m_Attack = false;
            m_Jump = false;
            m_HoldJump = false;
        }

        // Moves to a point.
        protected void PathAction() {
            m_Direction = (Vector2)(m_Path[m_PathIndex] - transform.position);
            m_Attack = false;
            m_Jump = false;
            m_HoldJump = false;
        }

        #endregion

    }
}
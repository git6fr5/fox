/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;

using Platformer.Decor;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class FallingSpike : Spike {

        /* --- Variables --- */
        #region Variables

        private static float FallGravity = 1f;
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public bool Falling => m_Body.gravityScale != 0f && m_Hitbox.enabled;

        [SerializeField] private bool m_Crumbling;
        [SerializeField] private float m_CrumbleBuffer;
        [SerializeField, ReadOnly] private float m_CrumbleTicks;

        [SerializeField] private float m_ShakeStrength;
        private float Strength => m_ShakeStrength * m_CrumbleTicks / m_CrumbleBuffer;

        public SparkleController m_Sparkle;
        
        #endregion
        
        void Awake() {
            Freeze();
        }

        private void Update() {
            if (!Falling) {
                m_Sparkle.Reset();
                m_Sparkle.enabled = false;
                CheckFall();
                Obstacle.Shake(transform, m_Origin, Strength);
            }
        }

        void FixedUpdate() {
            bool finished = Timer.TickUpIf(ref m_CrumbleTicks, m_CrumbleBuffer, Time.fixedDeltaTime, m_Crumbling);

            if (finished) {
                Fall();
                m_Crumbling = false;
                m_CrumbleTicks = 0f;
            }

        }

        protected override void ProcessCollision(Collider2D collider) {
            GroundCollision(collider);
            base.ProcessCollision(collider);
        }

        private void GroundCollision(Collider2D collider) {
            if (!Falling) {return;}
            bool hitGround = collider.gameObject.layer == LayerMask.NameToLayer("Ground");
            if (hitGround) {
                Shatter();
            }
        }

        private void CheckFall() {
            CharacterState character = CollisionCheck.LineOfSight<CharacterState>(transform.position, Vector3.down, Game.Physics.CollisionLayers.Opaque);
            m_Crumbling = character != null ? true : m_Crumbling;
        }

        private void Freeze() {
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_Body.gravityScale = 0f;
        }

        private void Fall() {
            m_Sparkle.enabled = true;
            transform.position = m_Origin + Game.Physics.CollisionPrecision * Direction;
            m_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_Body.gravityScale = FallGravity;
        }

        protected override void Shatter() {
            base.Shatter();
            Freeze();
            transform.position = m_Origin;
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;
using Platformer.Physics;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Spitball : MonoBehaviour {

        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public Rigidbody2D Body => m_Body;

        void FixedUpdate() {
            float angle = Vector2.SignedAngle(Vector2.down, m_Body.velocity);
            transform.eulerAngles = Vector3.forward * angle;
        }
        
        void OnTriggerEnter2D(Collider2D collider) {
            ProcessCollision(collider);
        }

        protected virtual void ProcessCollision(Collider2D collider) {
            CharacterCollision(collider);

            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                Shatter();
            }

            if (collider.gameObject.layer == LayerMask.NameToLayer("Platform")) {
                Shatter();
            }

        }

        private void CharacterCollision(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            if (character != null && character.IsPlayer) {
                character.Reset();
                Shatter();
            }
        }

        private void Shatter() {
            // if (m_ShatterEffect != null) {
            //     m_ShatterEffect.Play();
            // }
            // SoundManager.PlaySound(m_ShatterSound, 0.15f);
            Destroy(gameObject);
        }

    }
}
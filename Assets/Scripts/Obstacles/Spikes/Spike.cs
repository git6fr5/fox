/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;
using Platformer.Decor;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Spike : MonoBehaviour {

        /* --- Variables --- */
        #region Variables
        
        private static float ResetDelay = 2f;
        
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected BoxCollider2D m_Hitbox => GetComponent<BoxCollider2D>();
        public BoxCollider2D Hitbox => m_Hitbox;

        public Vector3 Rotation => new Vector3(0f, 0f, m_Rotation);
        public Vector3 Direction => Quaternion.Euler(0f, 0f, m_Rotation) * Vector3.up;
        
        [SerializeField] protected float m_KnockbackForce = 5f;
        [SerializeField] protected float m_Rotation = 0f;
        [SerializeField, ReadOnly] protected Vector3 m_Origin;

        [SerializeField] private VisualEffect m_ShatterEffect;
        [SerializeField] private AudioClip m_ShatterSound;
        [SerializeField] private VisualEffect m_RegrowEffect;
        [SerializeField] private AudioClip m_RegrowSound;
        
        #endregion

        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
            transform.eulerAngles = Rotation;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            ProcessCollision(collider);
        }

        protected virtual void ProcessCollision(Collider2D collider) {
            CharacterCollision(collider);
        }

        private void CharacterCollision(Collider2D collider) {
            CharacterState character = collider.GetComponent<CharacterState>();
            if (character != null && character.IsPlayer) {
                Vector3 knockbackDirection = GetKnockbackDirection(m_Rotation);
                bool didDamage = true; // character.Damage(1, knockbackDirection, m_KnockbackForce);
                character.Reset();
                if (didDamage) {
                    Shatter();
                }
            }
        }

        private static Vector3 GetKnockbackDirection(float rotation) {
            Vector3 direction = Quaternion.Euler(0f, 0f, rotation) * Vector3.up;
            direction.y = direction.y == 0f ? direction.y + 1f : direction.y;
            return direction.normalized;
        }

        protected virtual void Shatter() {
            if (m_ShatterEffect != null) {
                m_ShatterEffect.Play();
            }
            SoundManager.PlaySound(m_ShatterSound, 0.15f);
            m_Hitbox.enabled = false;
            m_SpriteRenderer.enabled = false;
            StartCoroutine(IEReset());
        }

        // Reset after a delay.
        IEnumerator IEReset() {
            float ratio = 7f / 16f;
            yield return new WaitForSeconds(ratio * ResetDelay);
            int count = 6;
            Color temp = m_SpriteRenderer.color;
            Color _temp = temp;
            _temp.a = 0.2f;
            m_SpriteRenderer.color = _temp; 
            for (int i = 0; i < count; i++) {
                _temp.a += 0.075f;
                m_SpriteRenderer.color = _temp; 
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                yield return new WaitForSeconds(ratio * ResetDelay / (float)count);

            }
            m_SpriteRenderer.color = temp;
            m_SpriteRenderer.enabled = true;

            if (m_RegrowEffect != null) {
                m_RegrowEffect.Play();
            }
            SoundManager.PlaySound(m_RegrowSound, 0.15f);

            yield return new WaitForSeconds(ResetDelay * (1f - 2f * ratio));
            Regrow();
            yield return null;
        }
        
        protected void Regrow() {
            m_Hitbox.enabled = true;
            m_SpriteRenderer.enabled = true;
        }

    }

}
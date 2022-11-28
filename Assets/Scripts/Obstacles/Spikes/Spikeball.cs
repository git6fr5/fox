/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Spikeball : MonoBehaviour {

        /* --- Variables --- */
        #region Variables
        
        private static float ResetDelay = 2f;
        
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();
        public CircleCollider2D Hitbox => m_Hitbox;

        [SerializeField] private VisualEffect m_ShatterEffect;
        [SerializeField] private AudioClip m_ShatterSound;
        [SerializeField] private VisualEffect m_RegrowEffect;
        [SerializeField] private AudioClip m_RegrowSound;

        protected Vector3 m_Origin;
        protected float m_Rotation;
        
        #endregion

        public void Init(int offset) {
            // 0 => peak down going up.
            // 1 => mid going up
            // 2 => peak up going down.
            // 3 => mid going down
            m_Rotation = 30f + (offset % 2) * 15f;
            m_Rotation *= offset >= 2 ? 1f : -1f;
            transform.eulerAngles = Vector3.forward * offset * 45f;
        }

        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
        }

        void FixedUpdate() {
            transform.eulerAngles += Vector3.forward * m_Rotation * Time.fixedDeltaTime;
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
                character.Reset();
                Shatter();
            }
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
            m_SpriteRenderer.color = Color.white;
        }

    }

}
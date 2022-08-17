/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Utilites;
using Platformer.Obstacles;
using Platformer.Character;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Orb : MonoBehaviour {

        private static float ResetDelay = 3.5f;

        public enum Type {
            DashOrb, HopOrb
        }

        [SerializeField] private Type m_Type;

        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();

        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        [SerializeField] private float m_Period = 3f;
        [SerializeField] private Vector2 m_Ellipse = new Vector2(0f, 2f/16f);

        [SerializeField] private VisualEffect m_CollectEffect;
        [SerializeField] private AudioClip m_CollectSound;
        [SerializeField] private VisualEffect m_RefreshEffect;
        [SerializeField] private AudioClip m_RefreshSound;

        void Start() {
            m_Origin = transform.position;
            m_Hitbox.isTrigger = true;
        }

        void FixedUpdate() {
            Timer.Cycle(ref m_Ticks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_Ticks, m_Period, m_Origin, m_Ellipse);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            CharacterState state = collider.GetComponent<CharacterState>();
            if (state != null && state.IsPlayer) {
                Collect(state);
            }
        }

        void Collect(CharacterState state) {
            // Game.HitStop(8);
            
            state.Dash.Enable(false);
            state.Hop.Enable(false);

            switch (m_Type) {
                case Type.DashOrb:
                    state.Dash.Enable(true);
                    break;
                case Type.HopOrb:
                    state.Hop.Enable(true);
                    break;
                default:
                    break;
            }

            if (m_CollectEffect != null) {
                m_CollectEffect.Play();
            }
            SoundManager.PlaySound(m_CollectSound, 0.15f);
            
            m_SpriteRenderer.enabled = false;
            m_Hitbox.enabled = false;
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

            if (m_RefreshEffect != null) {
                m_RefreshEffect.Play();
            }
            SoundManager.PlaySound(m_RefreshSound, 0.15f);

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
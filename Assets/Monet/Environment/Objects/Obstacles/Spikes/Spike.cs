/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Spike : MonoBehaviour {


        [HideInInspector] protected SpriteRenderer m_SpriteRenderer;
        [HideInInspector] protected BoxCollider2D m_Hitbox;
        
        [SerializeField] protected float m_KnockbackForce = 5f;
        [SerializeField] protected float m_Rotation = 0f;
        [SerializeField, ReadOnly] protected bool m_Active = false;
        [SerializeField, ReadOnly] protected Vector3 m_Target;

        void Start() {
            // Caching.
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Hitbox = GetComponent<BoxCollider2D>();
            // Initial values.
            m_Hitbox.isTrigger = true;
            m_Active = true;
            transform.eulerAngles = new Vector3(0f, 0f, m_Rotation);
            m_Target = transform.position;
            
            Outline.Add(m_SpriteRenderer, 0.5f, 16f);
            Outline.Set(m_SpriteRenderer, Color.black);
        }

        protected virtual void FixedUpdate() {
            float deltaTime = Time.fixedDeltaTime;
            transform.position += (m_Target - transform.position).normalized * deltaTime / 0.5f;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            Character character = collider.GetComponent<Character>();
            Player player = collider.GetComponent<Player>();
            if (character != null && player != null) {
                Vector3 direction = Quaternion.Euler(0f, 0f, m_Rotation) * Vector3.up;
                if (direction.y == 0f) {
                    direction.y += 1f;
                }
                direction = direction.normalized;
                bool didDamage = character.Damage(1, direction, m_KnockbackForce);
                if (didDamage) {
                    Outline.Set(m_SpriteRenderer, Color.white);
                    m_Hitbox.enabled = false;
                    StartCoroutine(IEHitbox());
                }
            }
        }
        
        protected IEnumerator IEHitbox() {
            yield return new WaitForSeconds(0.5f);
            Outline.Set(m_SpriteRenderer, Color.black);
            m_Hitbox.enabled = m_Active;
        }

        public void OnFlip() {
            Vector3 direction = Quaternion.Euler(0f, 0f, m_Rotation) * Vector3.up;
            m_Target += (m_Active ? -1f : 1f) * direction;
            m_Active = !m_Active;
            m_Hitbox.enabled = m_Active;
        }

    }

}
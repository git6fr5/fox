/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(ShieldWeapon))]
    public class ShieldProjectile : Projectile {

        private ShieldWeapon m_ShieldWeapon => GetComponent<ShieldWeapon>();

        [SerializeField] private float m_ReturnSpeed;
        [SerializeField] private float m_ReturnThreshold;
        [SerializeField] private AudioClip m_ReturnSound;
        [SerializeField, ReadOnly] private bool m_Return;
        public bool Returning => m_Return;

        void Awake() {
            m_Body.simulated = false;
            m_Body.gravityScale = 0f;
            // m_Hitbox.enabled = false;
            m_Return = false;
        }

        public void Return() {
            m_SpriteRenderer.sprite = m_FlashSprite;
            m_Hitbox.isTrigger = false;
            m_Return = true;
        }

        void LateUpdate() {
            if (m_Return) {
                Vector2 direction = (m_ShieldWeapon.ReturnTarget - transform.position).normalized;
                float distance = (m_ShieldWeapon.ReturnTarget - transform.position).magnitude;
                m_Body.velocity = m_ReturnSpeed * direction;
                if (distance < m_ReturnThreshold) {
                    Vector2 cache = m_Body.velocity;
                    Deactivate();
                    m_ShieldWeapon.Catch(cache);
                    m_Return = false;
                }
            }
        }

    }
}
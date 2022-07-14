/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Firefox : Projectile {

        [SerializeField] private Transform m_Parent;
        [SerializeField] private float m_Strength;

        void Start() {
            m_Parent = transform.parent;
            Controller controller = transform.parent.GetComponent<Character>()?.CharacterController;
            if (controller != null) {
                Init(Vector2.zero, controller.Targets);
            }
            transform.SetParent(null);
        }

        protected override void GroundCollision() {
            SoundManager.PlaySound(m_ImpactSound);
        }

        void FixedUpdate() {
            float deltaTime = Time.fixedDeltaTime;
            float distance = (m_Parent.position - transform.position).magnitude;
            Vector2 direction = (m_Parent.position - transform.position).normalized;
            m_Body.velocity += m_Strength * distance * direction * deltaTime;
            if (m_Body.velocity.magnitude > m_Speed) {
                m_Body.velocity = m_Body.velocity.normalized * m_Speed;
            }
        }

    }
}
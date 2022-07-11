/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Shield : Projectile {

        [SerializeField] private Transform m_Parent;
        [SerializeField] private float m_Strength;

        void Start() {
            m_Parent = transform.parent;
            Controller controller = transform.parent.GetComponent<Character>()?.CharacterController;
            if (controller != null) {
                Init(Vector2.zero, controller.Targets);
            }
            m_Body.simulated = false;
        }

        public override void Fire(bool fire, Vector2 direction, List<string> targets) {
            if (transform.parent != null) {
                if (fire && CanFire) {
                    Timer.CountdownTicks(ref m_Ticks, true, m_Cooldown, 0f);

                    transform.SetParent(null);
                    transform.position += (Vector3)direction * 0.9f;

                    m_Body.simulated = true;
                    Init(direction, targets);
                    m_Hitbox.isTrigger = false;
                    m_Hitbox.enabled = true;
                }
            }
        }

        protected override void ProcessCollision(Collider2D collider) {
            Character character = collider.GetComponent<Character>();
            CharacterCollision(character);
        }

        void FixedUpdate() {
            if (m_Body.simulated == false) {
                return;
            }

            Timer.CountdownTicks(ref m_Ticks, false, m_Cooldown, Time.fixedDeltaTime);
            if (m_Ticks > m_Cooldown - Time.fixedDeltaTime * 4f) {
                return;
            }
            
            float deltaTime = Time.fixedDeltaTime;
            float distance = (m_Parent.position - transform.position).magnitude;

            if (distance < 0.65f) {
                
                m_Body.velocity = Vector2.zero;
                m_Body.simulated = false;
                transform.SetParent(m_Parent);
                transform.localPosition = Vector3.zero;
                
                Timer.CountdownTicks(ref m_Ticks, true, 0f, 0f);

                return;
            }   

            if (m_Ticks > 0f) {
                return;
            }

            m_Hitbox.isTrigger = true;
            m_Hitbox.enabled = true;

            Vector2 direction = (m_Parent.position - transform.position).normalized;
            m_Body.velocity = 2f * m_Speed * direction;

        }

    }
}
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

        /* --- Variables --- */
        #region Variables

        [SerializeField] private Sparkle m_SmokeSparkle;
        [SerializeField] private AudioClip m_ReturnSound;
        [SerializeField] Vector3 m_Origin;
        
        // Settings.
        [SerializeField] private float m_ReturnSpeed;
        [SerializeField] private float m_ReturnThreshold;
        [SerializeField, ReadOnly] private bool m_Backswing;
        [SerializeField, ReadOnly] private Vector2 m_Direction;
        [SerializeField, ReadOnly] private float m_Power;
        [SerializeField] private float m_MaxPower;
        [SerializeField, ReadOnly] private Character m_Character;
        public Transform Node => m_Character.transform;
        public bool Attached => transform.parent == Node;
        public bool Active => !Attached && m_Body.simulated;
        public bool Return => Active && m_Ticks == 0f;
        public bool Swing => !m_Backswing && m_Power > 0f;
        
        #endregion

        void Start() {
            // Initialize the shield.
            m_Character = transform.parent.GetComponent<Character>();
            if (m_Character?.CharacterController != null) {
                Init(Vector2.zero, m_Character.CharacterController.Targets);
            }
            m_Origin = transform.localPosition;

            // Unsimulate the body.
            m_Body.simulated = false;
        }

        public override void Fire(Input input, bool fire, Vector2 direction, List<string> targets) {
            // if the projectile is currently active. 
            if (!Attached && fire) {
                Timer.CountdownTicks(ref m_Ticks, true, 0f, 0f);
                // input.ResetAttack();
            }
            // If this projectile is currently attached to the parent.
            else if (Attached && fire) {
                m_Backswing = true;
                m_Direction = direction;
            }
            else if (Attached && !fire) {
                m_Backswing = false;
            }

        }

        protected override void GroundCollision() {
            SoundManager.PlaySound(m_ImpactSound);
        }

        protected override void CharacterCollision(Character character) {
            if (character == null || character.CharacterState.Immune) {
                return;
            }
            else if (m_Targets.Contains(character.gameObject.tag)) {
                Vector2 knockbackDir = Quaternion.Euler(0f, 0f, 90f) * m_Body.velocity.normalized;
                knockbackDir.y = Mathf.Abs(knockbackDir.y);
                knockbackDir.y += 1f;
                character.Damage(m_Damage, knockbackDir, m_KnockbackForce);
                SoundManager.PlaySound(m_ImpactSound);
            }
        }

        void FixedUpdate() {

            if (!Active) {
                transform.localPosition = m_Origin;
            }

            if (m_Backswing) {
                m_Power += Time.fixedDeltaTime;
                if (m_Power > m_MaxPower) {
                    m_Power = m_MaxPower;
                }
                float flip = transform.eulerAngles.y >= 180f ? -1f : 1f;
                transform.localPosition = new Vector3(-flip * m_Direction.x, -m_Direction.y, 0f) * m_Power / m_MaxPower * m_ReturnThreshold;
            }
            
            if (Swing) {
                Throw();
            }

            // Don't move the projectile if its not active.
            if (!Active) { return; }

            // Incrememnt the ticks.
            Timer.CountdownTicks(ref m_Ticks, false, m_Cooldown, Time.fixedDeltaTime);
            if (Return) {

                if (!m_SmokeSparkle.gameObject.activeSelf) {
                    SoundManager.PlaySound(m_ReturnSound);
                    m_SmokeSparkle.Play();
                }
                
                // Reset the collider.
                m_Hitbox.isTrigger = true;
                m_Hitbox.enabled = true;

                // Move the body towars the node.
                Vector2 direction = (Node.position - transform.position).normalized;
                float distance = (Node.position - transform.position).magnitude;
                m_Body.velocity = m_ReturnSpeed * direction;

                if (distance < m_ReturnThreshold) {
                    // Reset the body.
                    m_Character.CharacterController.Knockback(m_Character.Body, m_Body.velocity.normalized * 5f, 0.1f);
                    m_Body.velocity = Vector2.zero;
                    m_Body.simulated = false;

                    // Reattach the projectile.
                    transform.SetParent(Node);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;

                    m_SmokeSparkle.Stop();

                }   

            }
            
        }

        private void Throw() {
            // Reset the timer to cooldown.
            Timer.CountdownTicks(ref m_Ticks, true, m_Power / m_MaxPower * m_Cooldown, 0f);

            // Dettach the projectile.
            transform.SetParent(null);
            transform.position += (Vector3)m_Direction * Game.Physics.MovementPrecision;

            // Active the body.
            m_Body.simulated = true;
            Init(m_Direction, m_Targets);

            // Enable the collider.
            m_Hitbox.isTrigger = false;
            m_Hitbox.enabled = true;

            // Play the effect.
            // m_ThrowSparkle;
            m_Power = 0f;
            m_Character.CharacterController.Knockback(m_Character.Body, -m_Direction.normalized * 5f, 0.1f);
            m_Character.CharacterInput.ResetAttack();

        }

    }
}
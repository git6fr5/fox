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

        [SerializeField] private AudioClip m_ReturnSound;
        [SerializeField] Vector3 m_Origin;
        
        // Settings.
        [SerializeField] private float m_ReturnSpeed;
        [SerializeField] private float m_ReturnThreshold;
        [SerializeField, ReadOnly] private bool m_Charge;
        [SerializeField, ReadOnly] private Vector2 m_Direction;
        [SerializeField, ReadOnly] private float m_Power;
        [SerializeField] private float m_MaxPower;
        [SerializeField, ReadOnly] private Character m_Character;
        public Transform Node => m_Character.transform;
        public bool Attached => transform.parent == Node;
        public bool Active => !Attached && m_Body.simulated;
        public bool Returning => Active && m_Ticks == 0f;
        public bool Swing => !m_Charge && m_Power > 0f;
        
        [SerializeField] private Sprite m_FlashSprite;
        [SerializeField] private float m_FlashDuration;
        [SerializeField, ReadOnly] private float m_FlashTicks;
        [HideInInspector] private Sprite m_BaseSprite;

        [SerializeField] private Color m_OutlineColor;

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

            // m_SpriteRenderer.materials = m_Materials;
            m_BaseSprite = m_SpriteRenderer.sprite;
            Outline.Add(m_SpriteRenderer, 0.5f, 16f);
            Outline.Set(m_SpriteRenderer, m_OutlineColor);
        }

        public override void Fire(Input input, bool fire, Vector2 direction, List<string> targets) {
            // if the projectile is currently active. 
            if (!Attached && fire) {
                Return();
            }
            // If this projectile is currently attached to the parent.
            else if (Attached && fire) {
                m_Charge = true;
                m_Direction = direction;
            }
            else if (Attached && !fire) {
                m_Charge = false;
            }

        }

        protected override void GroundCollision() {
            SoundManager.PlaySound(m_ImpactSound);
            Screen.Shake(0.075f, 0.1f);
            Flash();
        }

        public void Flash() {
            m_FlashTicks = m_FlashDuration;
            m_SpriteRenderer.sprite = m_FlashSprite;
            Outline.Set(m_SpriteRenderer, Color.white);
        }

        protected override void CharacterCollision(Character character) {
            if (m_Targets.Contains(character.gameObject.tag)) {
                Vector2 direction = GetKnockbackDirection(m_Body.velocity);
                bool didDamage = character.Damage(m_Damage, direction, m_KnockbackForce);
                if (didDamage) {
                    SoundManager.PlaySound(m_ImpactSound);
                    Flash();
                }
            }
        }

        void FixedUpdate() {

            bool finished = Timer.CountdownTicks(ref m_FlashTicks, false, 0f, Time.fixedDeltaTime);
            if (finished) {
                m_SpriteRenderer.sprite = m_BaseSprite;
                Outline.Set(m_SpriteRenderer, m_OutlineColor);
            }

            Idle();
            Charge();

            // Don't move the projectile if its not active.
            if (!Active) { return; }

            // Incrememnt the ticks.
            Timer.TickDown(ref m_Ticks, Time.fixedDeltaTime);
            if (Returning) {
                // Move the body towars the node.
                Vector2 direction = (Node.position - transform.position).normalized;
                float distance = (Node.position - transform.position).magnitude;
                m_Body.velocity = m_ReturnSpeed * direction;

                if (distance < m_ReturnThreshold) {
                    Catch();
                }

            }
            
        }

        private void Idle() {
            if (Active || m_Charge) { return; }

            transform.localPosition = m_Origin;
            Flipbook flipbook = m_Character.CharacterFlipbook;
            if (transform.parent != null && flipbook != null) {
                int frame = flipbook.CurrentFrame;
                int period = flipbook.AnimationLength;
                float flip = flipbook.transform.eulerAngles.y == 0f ? 1f : -1f;
                Vector3 origin = new Vector3(m_Origin.x * flip, m_Origin.y, 0f);
                Obstacle.Cycle(transform, frame + 2, period, transform.parent.position + origin, new Vector2(0.5f / 16f, 1f / 16f));
            }
        }

        private void Charge() {
            Timer.TickUpIf(ref m_Power, m_MaxPower, Time.fixedDeltaTime, m_Charge);
            if (!m_Charge && m_Power > 0f) {
                Throw();
            }
            if (m_Charge) {
                float flip = transform.eulerAngles.y >= 180f ? -1f : 1f;
                transform.localPosition = new Vector3(-flip * m_Direction.x, -m_Direction.y, 0f) * m_Power / m_MaxPower * m_ReturnThreshold;
            }
        }

        private void Return() {
            // Reset the collider.
            Timer.Reset(ref m_Ticks);
            m_SpriteRenderer.sprite = m_FlashSprite;
            m_Hitbox.isTrigger = true;
            m_Hitbox.enabled = true;
            // input.ResetAttack.
        }

        private void Catch() {
            m_Character.CharacterController.Knockback(m_Character.Body, m_Body.velocity.normalized * 5f, 0.1f);
            m_Body.velocity = Vector2.zero;
            m_Body.simulated = false;

            // Reattach the projectile.
            transform.SetParent(Node);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            m_SpriteRenderer.sprite = m_BaseSprite;
            // m_SmokeSparkle.Stop();
        }

        private void Throw() {
            // Reset the timer to cooldown.
            float ratio = m_Power > 0.08f ? m_Power / m_MaxPower : 0.05f;
            Timer.Start(ref m_Ticks, ratio * m_Cooldown);
            Timer.Reset(ref m_Power);

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
            m_Character.CharacterController.Knockback(m_Character.Body, -m_Direction.normalized * 5f, 0.1f);
            m_Character.CharacterInput.ResetAttack();
        }

        // Gets the knockback direction.
        private static Vector2 GetKnockbackDirection(Vector2 velocity) {
            Vector2 direction = Quaternion.Euler(0f, 0f, 60f) * velocity.normalized;
            // Adjust the y value.
            direction.y = Mathf.Abs(direction.y);
            direction.y += 1f;
            return direction;
        }

    }
}
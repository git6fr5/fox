/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Projectile : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        public enum CollisionType {
            Bounce,
            Stick,
            Explode,
            Delete
        }
        
        // Components.
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        protected Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        protected CircleCollider2D m_Hitbox => GetComponent<CircleCollider2D>();

        // Sprites.
        [HideInInspector] protected Sprite m_BaseSprite;
        [SerializeField] protected Sprite m_FlashSprite;
        [SerializeField] private float m_FlashDuration;
        [SerializeField, ReadOnly] private float m_FlashTicks;
        [SerializeField] protected Color m_OutlineColor;

        // Settings.
        [SerializeField] protected int m_Damage = 1;
        [SerializeField] protected float m_Speed = 10f;
        [SerializeField] protected float m_KnockbackForce = 5f;

        // Sounds.
        [SerializeField] private AudioClip m_ImpactSound;

        // Collision Settings.
        [HideInInspector] protected List<string> m_Targets;
        [SerializeField] private CollisionType m_OnGroundCollision;
        [SerializeField] private CollisionType m_OnCharacterCollision;
        
        #endregion

        // Runs once before the first frame.
        void Start() {
            gameObject.layer = LayerMask.NameToLayer("Projectiles");
            m_BaseSprite = m_SpriteRenderer.sprite;
            Outline.Add(m_SpriteRenderer, 0f, 16f);
            Outline.Set(m_SpriteRenderer, m_OutlineColor);
        }

        // Runs once every frame.
        void Update() {
            CharacterCollisions();
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            WhileFlashing(Time.fixedDeltaTime);
        }

        // Runs on every collision.
        void OnCollisionEnter2D(Collision2D collision) {
            ProcessCollision(collision.collider);
        }

        public void Activate(Vector2 direction, List<string> targets) {
            transform.SetParent(null);
            transform.position += (Vector3)direction.normalized * Game.Physics.MovementPrecision;
            m_Body.simulated = true;
            m_Hitbox.isTrigger = false;
            m_Body.velocity =  m_Speed * direction.normalized;
            m_Targets = targets;
        }

        public void Deactivate() {
            m_Body.velocity = Vector2.zero;
            m_Body.simulated = false;
            m_SpriteRenderer.sprite = m_BaseSprite;
            // m_Hitbox.enabled = false;
        }

        protected void ProcessCollision(Collider2D collider) {
            if (!m_Body.simulated) { return; }

            bool onGround = collider.gameObject.layer == LayerMask.NameToLayer("Ground");
            OnCollision(onGround, m_OnGroundCollision);
        }

        private void CharacterCollisions() {
            if (!m_Body.simulated) { return; }
            print("character collision");

            bool didDamage = false;
            List<Character> temp = PhysicsCheck.All<Character>(transform.position, m_Hitbox.radius, Game.Physics.CollisionLayers.Characters);
            List<Character> characters = temp.FindAll(character => m_Targets.Contains(character.gameObject.tag));
            for (int i = 0; i < characters.Count; i++) {
                Vector2 direction = Projectile.GetKnockbackDirection(m_Body.velocity);
                bool tempDamage = characters[i].Damage(m_Damage, direction, m_KnockbackForce);
                if (tempDamage) {
                    didDamage = true;
                }
            }
            OnCollision(didDamage, m_OnCharacterCollision);

        }

        protected void OnCollision(bool collision, CollisionType collisionType) {
            if (!collision) { return; }
            switch (collisionType) {
                case CollisionType.Bounce:
                    Bounce();
                    return;
                case CollisionType.Stick:
                    Stick();
                    return;
                case CollisionType.Explode:
                    Explode();
                    return;
                case CollisionType.Delete:
                    Delete();
                    return;
            }
        }

        public void Explode() {
            
        }

        public void Stick() {
            SoundManager.PlaySound(m_ImpactSound);
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            enabled = false;
        }

        public void Bounce() {
            SoundManager.PlaySound(m_ImpactSound);
            Screen.Shake(0.075f, 0.1f);
            StartFlash();
        }

        public void Delete() {
            SoundManager.PlaySound(m_ImpactSound);
            Screen.Shake(0.075f, 0.1f);
            Destroy(gameObject);
        }

        public void StartFlash() {
            Timer.Start(ref m_FlashTicks, m_FlashDuration);
            m_SpriteRenderer.sprite = m_FlashSprite;
            Outline.Set(m_SpriteRenderer, Color.white);
        }

        public void WhileFlashing(float deltaTime) {
            bool endflash = Timer.TickDown(ref m_FlashTicks, deltaTime);
            if (endflash) {
                m_SpriteRenderer.sprite = m_BaseSprite;
                Outline.Set(m_SpriteRenderer, m_OutlineColor);
            }
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
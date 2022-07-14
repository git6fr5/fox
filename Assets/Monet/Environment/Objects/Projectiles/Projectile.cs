/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // Components.
        [HideInInspector] protected Rigidbody2D m_Body;
        [HideInInspector] protected CircleCollider2D m_Hitbox;
        [HideInInspector] protected SpriteRenderer m_SpriteRenderer;

        // Details.
        public string Name => gameObject.name;
        [SerializeField] private string m_Description = "Insert description here...";
        public string Description => m_Description;

        // Settings.
        [SerializeField] protected int m_Damage = 1;
        [SerializeField] protected float m_Speed = 10f;
        [SerializeField] protected float m_KnockbackForce = 5f;
        [SerializeField] protected float m_Lifetime = 10f;
        // [SerializeField] private int m_Cost = 10;
        [SerializeField] protected float m_Torque = 0f;
        
        // Fire rate.
        [SerializeField, ReadOnly] protected float m_Ticks = 0f;
        [SerializeField] protected float m_Cooldown = 0.5f;
        public bool CanFire => m_Ticks == 0f;
        public float Cooldown => m_Cooldown;
        public float Ticks => m_Ticks;

        // Targets.
        [SerializeField, ReadOnly] protected List<string> m_Targets;

        // Sounds.
        [SerializeField] protected AudioClip m_ImpactSound;
        [SerializeField] protected AudioClip m_InitializeSound;
        
        #endregion

        /* --- Unity --- */
        #region Unity

        public void OnUpdate(float deltaTime) {
            Timer.CountdownTicks(ref m_Ticks, false, m_Cooldown, deltaTime);
        }

        void OnCollisionEnter2D(Collision2D collision) {
            ProcessCollision(collision.collider);
        }

        void Update() {
            if (!m_Body.simulated) {
                return;
            }

            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(transform.position, m_Hitbox.radius, Game.Physics.CollisionLayers.Characters);
            for (int i = 0; i < colliders.Length; i++) {
                Character character = colliders[i].GetComponent<Character>();
                CharacterCollision(character);
            }
        }
        
        protected void ProcessCollision(Collider2D collider) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                GroundCollision();
            }

            // Character character = collider.GetComponent<Character>();
            // CharacterCollision(character);

        }

        protected virtual void GroundCollision() {
            SoundManager.PlaySound(m_ImpactSound);
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            enabled = false;
        }

        protected virtual void CharacterCollision(Character character) {
            if (character == null) {
                return;
            }
            else if (m_Targets.Contains(character.gameObject.tag)) {
                character.Damage(m_Damage, m_Body.velocity, m_KnockbackForce);
                SoundManager.PlaySound(m_ImpactSound);
                Destroy(gameObject);
            }
        }

        #endregion

        /* --- Initialization --- */
        #region Initialization

        public virtual void Fire(Input input, bool fire, Vector2 direction, List<string> targets) {
            if (fire && CanFire) {
                Projectile projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
                projectile.Init(direction, targets);
                Timer.CountdownTicks(ref m_Ticks, true, m_Cooldown, 0f);
                input.ResetAttack();
            }
        }

        public void Init(Vector2 direction, List<string> targets) {
            // Caching.
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Body = GetComponent<Rigidbody2D>();
            m_Hitbox = GetComponent<CircleCollider2D>();

            // Set up.
            m_Body.gravityScale = 0f;
            m_Hitbox.isTrigger = true;
            transform.position += (Vector3)direction * (2f * (m_Hitbox.radius + Game.Physics.CollisionPrecision));
            m_Targets = targets;

            // Activate.
            gameObject.layer = LayerMask.NameToLayer("Projectiles");
            gameObject.SetActive(true);
            if (m_Lifetime > 0f) {
                Destroy(gameObject, m_Lifetime);
            }
            SoundManager.PlaySound(m_InitializeSound);
            // Physics.Shoot(m_Body, direction, m_Speed, m_Torque);
            
            m_Body.velocity = direction * m_Speed;
            m_Body.angularVelocity = m_Torque;
            transform.eulerAngles = Vector2.SignedAngle(Vector2.right, m_Body.velocity) * Vector3.forward;

        }
        
        #endregion

    }

}

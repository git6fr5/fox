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
        [SerializeField] private int m_Damage = 1;
        [SerializeField] protected float m_Speed = 10f;
        [SerializeField] private float m_KnockbackForce = 5f;
        [SerializeField] private float m_Lifetime = 10f;
        // [SerializeField] private int m_Cost = 10;
        [SerializeField] private float m_Torque = 0f;
        
        // Fire rate.
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        [SerializeField] private float m_Cooldown = 0.5f;
        public bool CanFire => m_Ticks == 0f;
        public float Cooldown => m_Cooldown;
        public float Ticks => m_Ticks;

        // Targets.
        [SerializeField, ReadOnly] private List<string> m_Targets;

        // Sounds.
        [SerializeField] private AudioClip m_ImpactSound;
        [SerializeField] private AudioClip m_InitializeSound;
        
        #endregion

        /* --- Unity --- */
        #region Unity

        public void OnUpdate(float deltaTime) {
            Timer.CountdownTicks(ref m_Ticks, false, m_Cooldown, deltaTime);
        }

        void OnTriggerEnter2D(Collider2D collider) {
            Character temp = collider.GetComponent<Character>();
            if (temp != null) {
                ProcessCollision(temp);
            }

            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
                enabled = false;
            }
        }
        
        private void ProcessCollision(Character character) {
            if (m_Targets.Contains(character.gameObject.tag)) {
                character.Damage(m_Damage, m_Body.velocity, m_KnockbackForce);
                SoundManager.PlaySound(m_ImpactSound);
                Destroy(gameObject);
            }
        }

        #endregion

        /* --- Initialization --- */
        #region Initialization

        public virtual void Fire(bool fire, Vector2 direction, List<string> targets) {
            if (fire && CanFire) {
                Projectile projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
                projectile.Init(direction, targets);
                Timer.CountdownTicks(ref m_Ticks, true, m_Cooldown, 0f);
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
            Destroy(gameObject, m_Lifetime);
            SoundManager.PlaySound(m_InitializeSound);
            // Physics.Shoot(m_Body, direction, m_Speed, m_Torque);
            
            m_Body.velocity = direction * m_Speed;
            m_Body.angularVelocity = m_Torque;
            transform.eulerAngles = Vector2.SignedAngle(Vector2.right, m_Body.velocity) * Vector3.forward;

        }
        
        #endregion

    }

}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    public string Name => gameObject.name;
    [SerializeField] private string m_Description = "Insert description here...";
    public string Description => m_Description;

    [SerializeField] private int m_Damage = 1;
    public int Damage => m_Damage;
    [SerializeField] private float m_Speed = 10f;
    public float Speed => m_Speed;
    [SerializeField] private float m_Lifetime = 10f;
    [SerializeField] private int m_Cost = 10;
    public float Cost => m_Cost;
    [SerializeField] private float m_Torque;
    
    [HideInInspector] private Rigidbody2D m_Body;
    [HideInInspector] private CircleCollider2D m_Hitbox;
    [HideInInspector] private SpriteRenderer m_SpriteRenderer;

    [SerializeField, ReadOnly] private float m_Ticks = 0f;
    [SerializeField] private float m_Cooldown = 0.5f;
    public bool CanFire => m_Ticks == 0f;
    public float Cooldown => m_Cooldown;
    public float Ticks => m_Ticks;

    [SerializeField, ReadOnly] private List<string> m_Targets;

    [SerializeField, ReadOnly] private bool m_IsHot = false;
    
    #endregion

    /* --- Unity --- */
    #region Unity
    
    void OnTriggerEnter2D(Collider2D collider) {
        if (!m_IsHot) {
            return;
        }

        Controller temp = collider.GetComponent<Controller>();
        if (temp != null) {
            ProcessCollision(temp);
        }

        Lever lever = collider.GetComponent<Lever>();
        if (lever != null) {
            ProcessCollision(lever);
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_IsHot = false;
            // m_SpriteRenderer.sortingLayerName = GameRules.BackgroundRenderingLayer;
            // m_SpriteRenderer.sortingOrder = -5;
        }
        
    }
    
    private void ProcessCollision(Controller controller) {
        // Target typee pls otherwise enemies will just kill each other and
        // the player will just murder themselvessss.
        if (m_Targets.Contains(controller.gameObject.tag)) {
            controller.Hurt(m_Damage);
            controller.Knockback(m_Body.velocity , 0.075f);
            Destroy(gameObject);
        }
    }

    private void ProcessCollision(Lever lever) {
        // Target typee pls otherwise enemies will just kill each other and
        // the player will just murder themselvessss.
        if (m_Targets.Contains("Switch")) {
            lever.Activate();
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_IsHot = false;
        }
    }

    public bool WillThisBeReadyToFireIn(float ticks) {
        return (m_Ticks - ticks) <= 0f;
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Fire(Vector3 direction, Vector2 referenceVelocity, List<string> targets) {
        if (CanFire) {
            Projectile projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
            projectile.Init(direction, referenceVelocity, targets);
            m_Ticks = m_Cooldown;
        }
    }

    public void UpdateCooldown(float deltaTime) {
        m_Ticks -= deltaTime;
        if (m_Ticks < 0f) {
            m_Ticks = 0f;
        }
    }

    public void Init(Vector3 direction, Vector2 referenceVelocity, List<string> targets) {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Body = GetComponent<Rigidbody2D>();
        m_Body.gravityScale = 0f;
        m_Hitbox = GetComponent<CircleCollider2D>();
        m_Hitbox.isTrigger = true;
        transform.position += direction * (2f * (m_Hitbox.radius + GameRules.MovementPrecision));

        // m_Map.gameObject.layer = LayerMask.NameToLayer("IgnoreRaycast");;
        
        m_IsHot = true;

        gameObject.SetActive(true);
        m_Body.velocity = (Vector2)direction * m_Speed; // + referenceVelocity;
        m_Body.angularVelocity = m_Torque;
        gameObject.layer = LayerMask.NameToLayer("Projectile");;
        m_Targets = targets;

        Destroy(gameObject, m_Lifetime);
    }
    
    #endregion

}
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

    [SerializeField] private float m_Damage = 1f;
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_Lifetime = 10f;
    
    [SerializeField] private Rigidbody2D m_Body;
    [SerializeField] private CircleCollider2D m_Hitbox;

    [SerializeField] private float m_Ticks = 0f;
    [SerializeField] private float m_Cooldown = 0.5f;
    public bool CanFire => m_Ticks == 0f;

    private int m_Collisions;
    
    #endregion

    /* --- Unity --- */
    #region Unity
    
    void OnCollisionEnter2D(Collision2D collision) {
        Controller temp = collision.gameObject.GetComponent<Controller>();
        if (temp != null) {
            ProcessCollision(temp);
        }
        m_Collisions += 1;
        if (m_Collisions >= 3) {
            Destroy(gameObject);
        }
    }
    
    private void ProcessCollision(Controller controller) {
        // Target typee pls otherwise enemies will just kill each other and
        // the player will just murder themselvessss.
        controller.Hurt(m_Damage);
        Destroy(gameObject);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Fire(Vector3 direction, Vector2 refvel) {
        if (CanFire) {
            Projectile projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
            projectile.Init(direction, refvel);
            m_Ticks = m_Cooldown;
        }
    }

    public void UpdateCooldown(float deltaTime) {
        m_Ticks -= deltaTime;
        if (m_Ticks < 0f) {
            m_Ticks = 0f;
        }
    }

    public void Init(Vector3 direction, Vector2 refvel) {
        m_Body = GetComponent<Rigidbody2D>();
        m_Body.gravityScale = 0f;
        m_Hitbox = GetComponent<CircleCollider2D>();
        m_Hitbox.isTrigger = false;
        transform.position += direction * (2f * (m_Hitbox.radius + GameRules.MovementPrecision));

        // m_Map.gameObject.layer = LayerMask.NameToLayer("IgnoreRaycast");;
        
        gameObject.SetActive(true);
        m_Body.velocity = (Vector2)direction * m_Speed + refvel;
        gameObject.layer = LayerMask.NameToLayer("Projectile");;

        Destroy(gameObject, m_Lifetime);
    }
    
    #endregion

}
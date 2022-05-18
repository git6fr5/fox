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

    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_Lifetime = 10f;
    
    [SerializeField] private Rigidbody2D m_Body;
    [SerializeField] private CircleCollider2D m_Hitbox;

    [SerializeField] private float m_Ticks = 0f;
    [SerializeField] private float m_Cooldown = 0.5f;
    public bool CanFire => m_Ticks == 0f;
    
    #endregion

    /* --- Unity --- */
    #region Unity
    
    void OnTriggerEnter2D(Collider2D collider) {
        Controller temp = collider.GetComponent<Controller>();
    }
    
    void OnTriggerExit2D(Collider2D collider) {
        Controller temp = collider.GetComponent<Controller>();
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Fire(Vector3 direction) {
        if (CanFire) {
            Projectile projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
            projectile.Init(direction);
            m_Ticks = m_Cooldown;
        }
    }

    public void UpdateCooldown(float deltaTime) {
        m_Ticks -= deltaTime;
        if (m_Ticks < 0f) {
            m_Ticks = 0f;
        }
    }

    public void Init(Vector3 direction) {
        m_Body = GetComponent<Rigidbody2D>();
        m_Body.gravityScale = 0f;
        m_Hitbox = GetComponent<CircleCollider2D>();
        m_Hitbox.isTrigger = true;
        transform.position += direction * (m_Hitbox.radius + GameRules.MovementPrecision);

        // m_Map.gameObject.layer = LayerMask.NameToLayer("IgnoreRaycast");;
        
        gameObject.SetActive(true);
        m_Body.velocity = direction * m_Speed;

        Destroy(gameObject, m_Lifetime);
    }
    
    #endregion

}
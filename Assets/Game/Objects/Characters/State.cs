/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[System.Serializable]
public class State {

    private Rigidbody2D body;
    private Transform transform;

    // Health.
    private int maxHealth;
    private int health;

    // Settings.
    private float height;
    private float width ;
    private float speed;
    private float acceleration;

    // Weapon.
    protected Projectile projectile;

    // Direction
    private float direction;
    private bool moving;

    // Jump.
    private float jumpforce;
    private float weight;
    private float floatiness;
    private bool onground;

    
    // Double Jump.
    private float doublejumpforce;
    private bool doublejumpreset;

    
    // Dash.
    private bool dashreset;
    private bool dashing;
    private float dashspeed;
    private float dashduration;
    public float DashDuration => m_DashDuration;

    public bool CanDash => m_DashReset && !m_Dashing;
    public bool CanDoubleJump => m_DoubleJumpReset && !m_OnGround;
    public bool CanJump => m_OnGround; // AirborneFlag == Airborne.Grounded;
    public bool Rising => m_Body.velocity.y > 0f;
    
    // Targets.
    protected List<string> targets;









    public void Init(Transform transform, Rigidbody2D body) {
        m_Health = m_MaxHealth;
        m_Body = body;
        m_Transform = transform;
    }

    public void FixedUpdate(float deltaTime) {
        m_Projectile.UpdateCooldown(deltaTime);
    }

    public void Update() {
        m_OnGround = CheckForGround(m_Transform, m_Width, m_Height);
        ResetDoubleJump();
    }

    public void SetMoving(float movement) {
        m_Moving = movement != 0f;
    }

    public void SetDirection(float direction) {
        m_Direction = direction;
    }

    public void StartDash() {
        dashing = true;
    }

    public void EndDash() {
        dashing = false;
    }

    private void DashReset() {
        if (CanJump && !m_DashReset) {
            m_DashReset = true;
        }
    }

    public void UseDoubleJump() {
        m_DoubleJumpReset = false;
    }

    private void ResetDoubleJump() {
        if (CanJump && !m_DoubleJumpReset) {
            m_DoubleJumpReset = true;
        }
    }

    private static bool CheckForGround(Transform transform, float width, float height) {
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = Vector3.down * height + i * Vector3.left * width / 1.5f;
            Collider2D temp = Physics2D.OverlapCircle(transform.position + offset, GameRules.MovementPrecision, GameRules.GroundCollisionLayer);
            if (temp != null) {
                return true;
            }
        }
        return false;
    }

    public void TakeDamage(int damage) {
        m_Health -= damage;
    }

    void OnDrawGizmos() {
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = Vector3.down * m_Height + i * Vector3.left * m_Width / 1.5f;
            Gizmos.DrawWireSphere(m_Transform.position + offset, 0.05f);
        }
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = i * m_Height / 1.5f * Vector3.up + m_Width * Vector3.right;
            Gizmos.DrawWireSphere(m_Transform.position + offset, 0.05f);
        }
    }

}
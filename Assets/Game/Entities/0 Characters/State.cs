/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[System.Serializable]
public class State {

    [HideInInspector] private Rigidbody2D m_Body;
    [HideInInspector] private Transform m_Transform;

    // Health.
    [SerializeField] private int m_MaxHealth;
    public int MaxHealth => m_MaxHealth;
    [SerializeField] private int m_Health;
    public int Health => m_Health;

    // Settings.
    [SerializeField] private float m_Height;
    public float Height => m_Height;
    [SerializeField] private float m_Width ;
    public float Width => m_Width;
    [SerializeField] private float m_Speed;
    public float Speed => m_Speed;
    [SerializeField] private float m_Acceleration;
    public float Acceleration => m_Acceleration;

    // Weapon.
    [SerializeField] protected Projectile m_Projectile;
    public Projectile Projectile => m_Projectile;

    // Direction
    [SerializeField, ReadOnly] private float m_Direction;
    public float Direction => m_Direction;
    [SerializeField, ReadOnly] private bool m_Moving;
    public bool Moving => m_Moving;

    // Jump.
    [SerializeField] private float m_JumpForce;
    public float JumpForce => m_JumpForce;
    [SerializeField] private float m_Weight;
    public float Weight => m_Weight;
    [SerializeField] private float m_Floatiness;
    public float Floatiness => m_Floatiness;
    [SerializeField, ReadOnly] private bool m_OnGround;
    public bool CanJump => m_OnGround; // AirborneFlag == Airborne.Grounded;
    public bool Rising => m_Body.velocity.y > 0f;
    
    // Double Jump.
    [SerializeField] private float m_DoubleJumpForce;
    public float DoubleJumpForce => m_DoubleJumpForce;
    [SerializeField, ReadOnly] private bool m_DoubleJumpReset;
    public bool DoubleJumpReset => m_DoubleJumpReset;
    public bool CanDoubleJump => m_DoubleJumpReset && !m_OnGround;
    
    // Dash.
    [SerializeField, ReadOnly] private bool m_DashReset;
    public bool CanDash => m_DashReset && !m_Dashing;
    [SerializeField, ReadOnly] private bool m_Dashing;
    public bool Dashing => m_Dashing;
    [SerializeField] private float m_DashSpeed;
    public float DashSpeed => m_DashSpeed;
    [SerializeField] private float m_DashDuration;
    public float DashDuration => m_DashDuration;

    // Ducking.
    // [SerializeField, ReadOnly] protected bool m_Ducked;
    // [SerializeField, ReadOnly] protected bool m_NotDucking;

    // public bool IsHot => m_State.Projectile != null && !m_State.Projectile.CanFire;

    // Targets.
    [SerializeField] protected List<string> m_Targets;
    public List<string> Targets => m_Targets;

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
        ResetDash();
    }

    public void SetMoving(float movement) {
        m_Moving = movement != 0f;
    }

    public void SetDirection(float direction) {
        m_Direction = direction;
    }

    public void StartDash() {
        m_Dashing = true;
    }

    public void EndDash() {
        m_Dashing = false;
    }

    private void ResetDash() {
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

    public void DebugCollisionChecks(Vector3 position) {
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = Vector3.down * m_Height + i * Vector3.left * m_Width / 1.5f;
            Gizmos.DrawWireSphere(position + offset, 0.05f);
        }
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = i * m_Height / 1.5f * Vector3.up + m_Width * Vector3.right;
            Gizmos.DrawWireSphere(position + offset, 0.05f);
        }
    }

}
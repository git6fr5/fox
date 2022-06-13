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

    // Coins.
    [SerializeField] private int m_Gold;
    public int Gold => m_Gold;

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
    public bool IsHot => !m_Projectile.CanFire;

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
    public bool CanJump => m_OnGround && !m_InWater; // AirborneFlag == Airborne.Grounded;
    public bool Rising => m_Body.velocity.y > 0f;
    
    // Double Jump.
    [SerializeField] private float m_DoubleJumpForce;
    public float DoubleJumpForce => m_DoubleJumpForce;
    [SerializeField, ReadOnly] private bool m_DoubleJumpReset;
    public bool DoubleJumpReset => m_DoubleJumpReset;
    public bool CanDoubleJump => m_DoubleJumpReset && !m_OnGround && !m_InWater;
    
    // Dash.
    [SerializeField, ReadOnly] private bool m_DashReset;
    public bool CanDash => m_DashReset && !m_Dashing;
    [SerializeField, ReadOnly] private bool m_Dashing;
    public bool Dashing => m_Dashing;
    [SerializeField] private float m_DashSpeed;
    public float DashSpeed => m_DashSpeed;
    [SerializeField] private float m_DashDuration;
    public float DashDuration => m_DashDuration;

    // Climb
    [SerializeField] private float m_ClimbSpeed;
    public float ClimbSpeed => m_ClimbSpeed;
    [SerializeField, ReadOnly] private bool m_Climbing;
    public bool Climbing => m_Climbing;
    [SerializeField, ReadOnly] private bool m_FacingWall;
    public bool CanClimb => m_FacingWall;
    [SerializeField] private float m_WallJumpForce;
    public float WallJumpForce => m_WallJumpForce;
    [SerializeField] private bool m_WallJumping;
    public bool WallJumping => m_WallJumping;

    // Swimming
    [SerializeField] private bool m_InWater;
    public bool Swimming => m_InWater;
    [SerializeField] private float m_SwimSpeed = 12f;
    public float SwimSpeed => m_SwimSpeed;
    [SerializeField] private float m_SwimAcceleration = 50f;
    public float SwimAcceleration => m_SwimAcceleration;
    [SerializeField] private float m_SwimResistance = 0.995f;
    public float SwimResistance => m_SwimResistance;

    // Ducking.
    [SerializeField, ReadOnly] protected bool m_Ducking;
    public bool Ducking => m_Ducking;
    [SerializeField, ReadOnly] protected bool m_Ducked;
    public bool CanDuck => m_OnGround;
    [SerializeField, ReadOnly] protected bool m_NotDucking;

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
        if (m_Projectile != null) {
            m_Projectile.UpdateCooldown(deltaTime);
        }
    }

    public void Update() {
        m_OnGround = CheckForGround(m_Transform, m_Width, m_Height);
        m_InWater = CheckForWater(m_Transform);
        m_FacingWall = CheckForWall(m_Transform, m_Direction, m_Width, m_Height);
        ResetDoubleJump();
        ResetDash();
        EndClimbing();
    }

    public void SetMoving(float movement) {
        m_Moving = movement != 0f;
    }

    public void SetDirection(float direction) {
        m_Direction = direction;
    }

    public void StartDash() {
        m_Dashing = true;
        m_DashReset = false;
    }

    public void EndDash() {
        m_Dashing = false;
    }

    private void ResetDash() {
        if (CanJump && !m_DashReset && !m_InWater) {
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

    public void StartClimbing() {
        m_Climbing = true;
    }

    public void EndClimbing() {
        if (m_Climbing && !m_FacingWall) {
            m_Climbing = false;
        }
    }

    public void StartWallJump() {
        m_Climbing = false;
        m_WallJumping = true;
    }

    public void EndWallJump(bool continueJump) {
        if (!continueJump) {
            m_WallJumping = false;
        }
        else if (m_OnGround || m_InWater) {
            m_WallJumping = false;
        }

        // Wall to Wall jump.
        if (m_WallJumping && m_FacingWall) {
            m_Climbing = true;
            m_WallJumping = false;
        }
    }

    public void StartDuck() {
        m_Ducking = true;
        m_Ducked = false;
    }

    public void EndDuck(bool continueDuck) {
        if (!continueDuck) {
            m_Ducking = false;
            return;
        }

        if (m_Ducking && !m_Ducked) {
            m_Ducked = !m_OnGround && !Rising;
        }

        if (m_Ducked && m_OnGround) {
            m_Ducking = false;
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

    private static bool CheckForWall(Transform transform, float direction, float width, float height) {
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = i * height / 1.5f * Vector3.up + direction * width * Vector3.right;
            Collider2D temp = Physics2D.OverlapCircle(transform.position + offset, GameRules.MovementPrecision, GameRules.WallCollisionLayer);
            if (temp != null) {
                return true;
            }
        }
        return false;
    }

    private static bool CheckForWater(Transform transform) {
        Collider2D temp = Physics2D.OverlapCircle(transform.position, GameRules.MovementPrecision, GameRules.WaterCollisionLayer);
        if (temp != null) {
            return true;
        }
        return false;
    }

    public void TakeDamage(int damage) {
        m_Health -= damage;
    }

    public void AddGold(int value) {
        m_Gold += value;
    }

    public bool LoseGold(int value) {
        if (m_Gold >= value) {
            m_Gold -= value;
            return true;
        }
        return false;
    }

    public void DebugCollisionChecks(Vector3 position) {
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = Vector3.down * m_Height + i * Vector3.left * m_Width / 1.5f;
            Gizmos.DrawWireSphere(position + offset, 0.05f);
        }
        for (int i = -1; i <= 1; i++) {
            Vector3 offset = i * m_Height / 1.5f * Vector3.up + m_Width * m_Direction * Vector3.right;
            Gizmos.DrawWireSphere(position + offset, 0.05f);
        }
    }

}
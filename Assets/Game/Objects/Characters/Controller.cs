/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the actions of a character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class Controller : MonoBehaviour {

    /* --- Enumerations --- */
    #region Enumerations

    public enum Direction {
        Right,
        Left
    }

    public enum Movement {
        Idle,
        Moving
    }

    public enum Airborne {
        Grounded,
        Rising,
        Falling,
        Landing
    }

    #endregion

    /* --- Variables --- */
    #region Variables
    
    // Components.
    [HideInInspector] protected Rigidbody2D m_Body; // Handles physics calculations.
    public Rigidbody2D Body => m_Body;

    // Health.
    [SerializeField] protected int m_MaxHealth = 3;
    [SerializeField] protected int m_Health;
    public int MaxHealth => m_MaxHealth;
    public int Health => m_Health;

    // Settings.
    [Space(2), Header("Settings")]
    [SerializeField] protected float m_Height = 1f;
    [SerializeField] protected float m_Width = 0.5f;
    [SerializeField] protected float m_Speed;
    [SerializeField] protected float m_Acceleration;
    [SerializeField] protected float m_JumpForce;
    [SerializeField] protected float m_Weight;
    [SerializeField] protected float m_Floatiness;

    // Controls
    [Space(2), Header("Controls")]
    [SerializeField, ReadOnly] protected float m_MoveInput;
    [SerializeField, ReadOnly] protected bool m_JumpInput;
    [SerializeField, ReadOnly] protected bool m_AttackInput;
    [SerializeField, ReadOnly] protected bool m_FloatInput;

    // Knockback.
    [SerializeField, ReadOnly] protected float m_KnockbackTicks;
    public bool m_Knockedback => m_KnockbackTicks > 0f;
    
    // Ducking.
    [SerializeField, ReadOnly] protected bool m_DuckInput;
    [SerializeField, ReadOnly] protected bool m_Ducked;
    [SerializeField, ReadOnly] protected bool m_NotDucking;

    // Flags.
    [Space(2), Header("Flags")]
    [SerializeField, ReadOnly] public Movement MovementFlag = Movement.Idle; 
    [SerializeField, ReadOnly] public Direction DirectionFlag = Direction.Right;
    [SerializeField, ReadOnly] public Airborne AirborneFlag = Airborne.Grounded;

    // Debug.
    [Space(2), Header("Debug")]
    [HideInInspector] private Vector3 m_PreviousPosition;
    [SerializeField, ReadOnly] private float m_DebugSpeed;
    [SerializeField, ReadOnly] private float m_DebugJumpHeight;
    [SerializeField, ReadOnly] private float m_DebugJumpDistance;
    [SerializeField, ReadOnly] private Vector3 m_DebugJumpStartPosition;

    // Combat.
    [SerializeField] protected Projectile m_Projectile;
    public Projectile projectile => m_Projectile;
    public bool IsHot => m_Projectile != null && !m_Projectile.CanFire;
    [SerializeField] protected List<string> m_Targets;
    [SerializeField, ReadOnly] protected Vector2 m_AttackDirection;
    
    #endregion

    /* --- Unity --- */
    #region Unity 

    private void Start() {
        Init();
    }

    private void Update() {
        if (m_Knockedback) { return; }
        
        GetInput();
        GetFlags();
        ProcessJump();
        ProcessDuck();
        ProcessAttack();
    }

    // Runs once every fixed interval.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        ProcessThink(deltaTime);
        
        if (m_Knockedback) { return; }
        ProcessMovement(deltaTime);
        DebugMovement(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    protected void Init() {
        m_Body = GetComponent<Rigidbody2D>();
        m_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        m_PreviousPosition = transform.position;
        m_Health = m_MaxHealth;
    }

    #endregion

    /* --- Actions --- */
    #region Actions
    
    protected abstract void GetInput();

    protected virtual void ProcessMovement(float deltaTime) {
        float target = m_Speed * m_MoveInput;
        if (Mathf.Abs(target - m_Body.velocity.x) >= m_Acceleration * deltaTime) {
            float deltaVelocity = Mathf.Sign(target - m_Body.velocity.x) * m_Acceleration * deltaTime;
            m_Body.velocity = new Vector2(m_Body.velocity.x + deltaVelocity, m_Body.velocity.y);
        }
        else {
            m_Body.velocity = new Vector2(target, m_Body.velocity.y);
        }
    }

    protected virtual void ProcessJump() {
        m_Body.gravityScale = m_Weight * GameRules.GravityScale;
        if (m_JumpInput && AirborneFlag == Airborne.Grounded) {
            m_Body.velocity = new Vector2(m_Body.velocity.x, m_JumpForce);
            AirborneFlag = Airborne.Rising;
        }

        if (m_FloatInput && AirborneFlag == Airborne.Rising) {
            m_Body.gravityScale *= m_Floatiness;
        }

    }

    protected void ProcessDuck() {
        if (m_DuckInput) {
            gameObject.layer = LayerMask.NameToLayer("Ducking");
        }
        else {
            gameObject.layer = LayerMask.NameToLayer("Characters");
        }
    }

    protected void ProcessAttack() {
        if (m_AttackInput) {
            m_Projectile.Fire(m_AttackDirection, m_Body.velocity, m_Targets);
        }
    }

    protected virtual void ProcessThink(float deltaTime) {
        m_Projectile.UpdateCooldown(deltaTime);
        if (m_KnockbackTicks > 0f) {
            m_KnockbackTicks -= deltaTime;
        }
    }
    
    #endregion

    /* --- Flags --- */
    #region Flags

    private void GetFlags() {
        GetDirectionFlag();
        GetMovementFlag();
        GetAirborneFlag();
    }

    private void GetDirectionFlag() {
        if (m_MoveInput != 0) {
            DirectionFlag = (m_MoveInput > 0) ? Direction.Right : Direction.Left;
        }
    }

    private void GetMovementFlag() {
        MovementFlag = Movement.Idle;
        if (m_MoveInput != 0) {
            MovementFlag = Movement.Moving;
        }
    }

    private void GetAirborneFlag() {

        Collider2D ground = null;
        for (int i = -1; i <= 1; i++) {
            Collider2D temp = Physics2D.OverlapCircle(transform.position + Vector3.down * m_Height + i * Vector3.left * m_Width / 1.5f, GameRules.MovementPrecision, GameRules.GroundCollisionLayer);
            if (temp != null) {
                ground = temp;
            }
        }

        if (ground == null) {
            AirborneFlag = m_Body.velocity.y < 0 ? Airborne.Falling : Airborne.Rising;
        }
        else {
            if (m_Body.velocity.y > 0 && m_FloatInput == true) {
                AirborneFlag = Airborne.Rising;
                return;
            }
            AirborneFlag = Airborne.Grounded;
        }
    }

    #endregion

    /* --- Debug --- */
    #region Debug

    void OnDrawGizmos() {
        if (GameRules.Instance != null) {
            Gizmos.DrawWireSphere(transform.position + Vector3.down * m_Height, GameRules.MovementPrecision);
        }
        for (int i = -1; i <= 1; i++) {
            Gizmos.DrawWireSphere(transform.position + Vector3.down * m_Height + i * Vector3.left * m_Width / 1.5f, 0.05f);
        }
        for (int i = -1; i <= 1; i++) {
            Gizmos.DrawWireSphere(transform.position + i * m_Height / 1.5f * Vector3.up + m_Width * Vector3.right, 0.05f);
        }
    }

    private void DebugMovement(float deltaTime) {
        DebugSpeed(deltaTime);
        DebugJump();
    }

    private void DebugSpeed(float deltaTime) {
        m_DebugSpeed = Mathf.Max(m_DebugSpeed, Mathf.Abs((transform.position.x - m_PreviousPosition.x)) / deltaTime);
        m_PreviousPosition = transform.position;
    }

    private void DebugJump() {
        if (AirborneFlag != Airborne.Grounded) {
            if (AirborneFlag == Airborne.Rising) {
                m_DebugJumpHeight = transform.position.y - m_DebugJumpStartPosition.y;
            }
            m_DebugJumpDistance = Mathf.Abs(transform.position.x - m_DebugJumpStartPosition.x);
        } else {
            m_DebugJumpStartPosition = transform.position;
        }
    }

    #endregion

    /* --- Health --- */
    #region Health

    public void Hurt(int damage) {
        if (m_Knockedback) { return; }

        m_Health -= damage;
        if (m_Health <= 0) {
            Kill();
        }
    }

    public virtual void Knockback(Vector2 velocity, float duration) {
        m_Body.velocity = velocity;
        m_KnockbackTicks = duration;
    }

    public void Kill() {
        Destroy(gameObject);
    }

    #endregion

}

﻿/* --- Libraries --- */
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

    // Settings.
    [Space(2), Header("Settings")]
    [SerializeField] protected float m_Height;
    [SerializeField] protected float m_Speed;
    [SerializeField] protected float m_Acceleration;
    [SerializeField] protected float m_JumpForce;
    [SerializeField] protected float m_Weight;
    [SerializeField] protected float m_Floatiness;

    // Controls
    [Space(2), Header("Controls")]
    [SerializeField, ReadOnly] protected float m_MoveInput;
    [SerializeField, ReadOnly] protected bool m_JumpInput;
    [SerializeField, ReadOnly] protected bool m_FloatInput;

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
    
    #endregion

    /* --- Unity --- */
    #region Unity 

    private void Start() {
        Init();
    }

    private void Update() {
        GetInput();
        GetFlags();
        ProcessJump();
    }

    // Runs once every fixed interval.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        ProcessThink(deltaTime);
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
        if (m_FloatInput && AirborneFlag == Airborne.Rising) {
            m_Body.gravityScale *= m_Floatiness;
        }
        if (m_JumpInput && AirborneFlag == Airborne.Grounded) {
            m_Body.velocity = new Vector2(m_Body.velocity.x, m_JumpForce);
        }
    }

    protected virtual void ProcessThink(float deltaTime) {
        
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
        Collider2D ground = Physics2D.OverlapCircle(transform.position + Vector3.down * m_Height, GameRules.MovementPrecision, GameRules.GroundCollisionLayer);
        if (ground == null) {
            AirborneFlag = m_Body.velocity.y < 0 ? Airborne.Falling : Airborne.Rising;
        }
        else {
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

}

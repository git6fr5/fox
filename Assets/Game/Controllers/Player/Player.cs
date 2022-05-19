/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    #region Input

    public static float m_ClimbBufferDuration = 0.15f;

    [SerializeField, ReadOnly] private KeyCode m_JumpKey = KeyCode.Space;

    // Climbing.
    [SerializeField, ReadOnly] private float m_ClimbInput = 0f;
    [SerializeField, ReadOnly] private float m_ClimbDirection = 0f;
    [SerializeField, ReadOnly] private bool m_WallClimbing = false;
    [SerializeField, ReadOnly] private bool m_WallJump = false;
    [SerializeField, ReadOnly] private bool m_WallJumping = false;
    [SerializeField] private float m_Width = 0.25f;
    [SerializeField, ReadOnly] private bool m_CanClimb = false;
    [SerializeField, ReadOnly] private float m_WallClimbTicks = 0f;
    [SerializeField] private float m_WallClimbDuration = 1f;
    [SerializeField, ReadOnly] private float m_ClimbBufferTicks = 0f;
    [SerializeField] private float m_WallJumpForce = 0f;
    [SerializeField] private float m_WallClimbSpeed = 0f;
    public bool WallClimbing => m_WallClimbing;
    public bool WallJumping => m_WallJumping;
    public float ClimbInput => m_ClimbInput;

    // Dash.
    [SerializeField, ReadOnly] private Vector2 m_DashInput = Vector2.zero;
    [SerializeField, ReadOnly] private bool m_CanDash = false;
    [SerializeField, ReadOnly] private bool m_Dashing = false;
    [SerializeField, ReadOnly] private bool m_EndDash = false;
    [SerializeField] private float m_DashSpeed = 40f;
    [SerializeField] private float m_DashDuration = 0.5f;
    public bool Dashing => m_Dashing;

    // Double Jump.
    [SerializeField, ReadOnly] private bool m_DoubleJumpInput = false;
    [SerializeField, ReadOnly] private bool m_CanDoubleJump = false;
    [SerializeField] private float m_DoubleJumpForce = 20f;
    [SerializeField, ReadOnly] private bool m_DoubleJumping = false;
    public bool DoubleJumping => m_DoubleJumping;

    // Swim.
    [SerializeField] private bool m_Swimming = false;
    [SerializeField, ReadOnly] private Vector2 m_SwimMoveInput = Vector2.zero;
    [SerializeField] private float m_SwimSpeed = 12f;
    [SerializeField] private float m_SwimAcceleration = 50f;
    [SerializeField] private float m_SwimResistance = 0.995f;

    // Attack.

    protected override void GetInput() {

        // Basic.
        m_MoveInput = Input.GetAxisRaw("Horizontal");

        // Jumping.
        m_JumpInput = Input.GetKeyDown(m_JumpKey);
        m_FloatInput = Input.GetKeyDown(m_JumpKey) ? true : (Input.GetKeyUp(m_JumpKey) ? false : m_FloatInput);

        // Ducking.
        // m_DuckInput = Input.GetAxisRaw("Vertical") == -1f;
        m_DuckInput = (Input.GetAxisRaw("Vertical") == -1f && m_NotDucking) && AirborneFlag == Airborne.Grounded ? true : (Input.GetKeyUp(KeyCode.S) || (m_Ducked) ? false : m_DuckInput);
        m_Ducked = m_DuckInput && AirborneFlag == Airborne.Falling;
        m_NotDucking = Input.GetAxisRaw("Vertical") != -1f;

        // Climbing.
        m_ClimbInput = Input.GetAxisRaw("Vertical");
        m_CanClimb = m_CanClimb ? CheckFall() : CheckClimb();
        m_WallClimbing = m_CanClimb && (m_WallClimbing ? true : (m_ClimbInput != 0f ? true : false));
        m_WallJump = !m_WallClimbing ? false : (Input.GetKeyDown(m_JumpKey) ? true : false);
        m_WallJumping = m_Dashing || AirborneFlag == Airborne.Grounded ? false : (m_WallJump ? true : (Input.GetKeyUp(m_JumpKey) ? false : m_WallJumping));

        // Dashing.
        // m_DashInput = (AirborneFlag != Airborne.Grounded && Input.GetKeyDown(m_JumpKey)) ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))  : Vector2.zero;
        float y = Input.GetAxisRaw("Vertical");
        float x1 = DirectionFlag == Direction.Right ? 1f : -1f;
        float x2 = Input.GetAxisRaw("Horizontal");
        Vector2 directionalInput = y == 0f ? new Vector2(x1, 0f) : new Vector2(x2, y);
        m_DashInput = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K) ? (directionalInput == Vector2.zero ? new Vector2(0f, 1f) : directionalInput) : Vector2.zero;
        m_CanDash = m_WallClimbing || m_Dashing ? false : ((m_WallJumping || AirborneFlag == Airborne.Grounded) ? true : m_CanDash);
        m_EndDash = !m_Dashing ? false : (Input.GetKeyUp(m_JumpKey) ? true : m_EndDash);

        // Swimming
        // m_Swimming = true;
        m_SwimMoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Flying

        // Attacking
        m_AttackInput = Input.GetMouseButtonDown(0);
        m_AttackDirection = (Screen.MousePosition - (Vector2)transform.position).normalized;

        // Double Jump.
        m_DoubleJumpInput = (AirborneFlag != Airborne.Grounded && Input.GetKeyDown(m_JumpKey)) ? true  : false;
        m_CanDoubleJump = m_WallClimbing || m_Dashing ? false : ((m_WallJumping || AirborneFlag == Airborne.Grounded) ? true : m_CanDoubleJump);
        m_DoubleJumping = m_DoubleJumpInput && m_CanDoubleJump ? true : (m_WallClimbing || m_Dashing || AirborneFlag == Airborne.Grounded ? false : m_DoubleJumping);
    }

    protected override void ProcessMovement(float deltaTime) {
        if (m_Swimming) {
            ProcessSwimMovement(deltaTime);
        }
        else if (m_Dashing) {
            // Do nothing.
        }
        else if (m_WallClimbing && !m_WallJumping) {
            ProcessWallClimbingMovement(deltaTime);
        } else if (m_WallJumping) {
            // Do nothing.
        }
        else {
            base.ProcessMovement(deltaTime);
        }
    }

    protected override void ProcessJump() {
        if (m_CanDash && m_DashInput != Vector2.zero) {
            ProcessDash();
        }
        else if (m_Dashing) {
            // 
        }
        else if (m_WallClimbing) {
            ProcessWallJump();
        }
        else if (m_DoubleJumpInput && m_CanDoubleJump) {
            ProcessDoubleJump();
        } else {
            base.ProcessJump();
        }
    }

    private void ProcessDoubleJump() {
        m_Body.velocity = new Vector2(m_Body.velocity.x, m_DoubleJumpForce);
        m_CanDoubleJump = false;
    }

    protected override void ProcessThink(float deltaTime) {
        if (m_WallClimbing) {
            m_WallClimbTicks += deltaTime;
        }
        if (m_CanClimb) {
            m_ClimbBufferTicks += deltaTime;
        }
        base.ProcessThink(deltaTime);
    }

    /* --- Climbing --- */
    #region Climbing

    private bool CheckClimb() {
        Vector3 direction = DirectionFlag == Direction.Right ? Vector3.right : Vector3.left;
        Collider2D ground = Physics2D.OverlapCircle(transform.position + direction * m_Width, GameRules.MovementPrecision, GameRules.ClimbCollisionLayer);
        if (ground != null) {
            m_ClimbDirection = direction.x;
            return true;
        }
        return false;
    }

    private bool CheckFall() {
        Vector3 direction = DirectionFlag == Direction.Right ? Vector3.right : Vector3.left;
        Collider2D ground = Physics2D.OverlapCircle(transform.position + direction * m_Width, GameRules.MovementPrecision, GameRules.GroundCollisionLayer);
        if (ground != null) {
            m_ClimbBufferTicks = 0f;
        }

        if (m_WallJumping || m_ClimbBufferTicks >= m_ClimbBufferDuration) {
            return false;
        }
        return true;
    }

    private void ProcessWallClimbingMovement(float deltaTime) {
        float target = m_WallClimbSpeed * m_ClimbInput;
        if (Mathf.Abs(target - m_Body.velocity.y) >= m_Acceleration * deltaTime) {
            float deltaVelocity = Mathf.Sign(target - m_Body.velocity.y) * m_Acceleration * deltaTime;
            m_Body.velocity = new Vector2(0f, m_Body.velocity.y + deltaVelocity);
        } else {
            m_Body.velocity = new Vector2(0f, target);
        }
    }

    private void ProcessWallJump() {
        m_Body.gravityScale = 0f;
        if (m_WallJump) {
            float wallJumpDirection = m_ClimbDirection;
            m_Body.velocity = m_WallJumpForce * (new Vector2(-wallJumpDirection, 1f)).normalized;
        }
    }

    #endregion

    /* --- Dashing --- */
    #region Dashing
    
    private void ProcessDash() {
        m_Dashing = true;
        m_Body.velocity = Vector2.zero;
        m_Body.gravityScale = 0f;
        StartCoroutine(IEDash(m_DashDuration, m_DashInput));

        IEnumerator IEDash(float duration, Vector2 direction) {
            yield return new WaitForSeconds(0.05f);
            m_Body.velocity = direction.normalized * m_DashSpeed;
            int m_Precision = 6;
            for (int i = 0; i < m_Precision; i++) {
                // if (i > 0 && m_EndDash) {
                //     break;
                // }
                yield return new WaitForSeconds(duration / (float)m_Precision);
            }
            m_Dashing = false;
            yield return null;
        }

    }
    
    #endregion

    /* --- Swimming --- */
    #region Swimming
    
    private void ProcessSwimMovement(float deltaTime) {
        // Process the physics.
        Vector2 targetVelocity = (Vector3)m_SwimMoveInput.normalized * m_SwimSpeed;
        Vector2 deltaVelocity = (targetVelocity - m_Body.velocity) * m_SwimAcceleration * deltaTime;
        m_Body.velocity += deltaVelocity;
        // Resistance
        if (targetVelocity == Vector2.zero) {
            m_Body.velocity *= m_SwimResistance;
        }
        // Check for released inputs.
        if (targetVelocity.y == 0f && Mathf.Abs(m_Body.velocity.y) < GameRules.MovementPrecision) {
            m_Body.velocity = new Vector2(m_Body.velocity.x, 0f);
        }
        if (targetVelocity.x == 0f && Mathf.Abs(m_Body.velocity.x) < GameRules.MovementPrecision) {
            m_Body.velocity = new Vector2(0f, m_Body.velocity.y);
        }
    }
    
    #endregion

    #endregion

    #region Level
    
    [SerializeField] private Level m_Level;
    public Level level => m_Level;

    [SerializeField] private Checkpoint m_Checkpoint;
    public Checkpoint checkpoint => m_Checkpoint;

    public void SetLevel(Level level) {
        m_Level = level;
    }

    public void SetCheck(Checkpoint checkpoint) {
        m_Checkpoint = checkpoint;
    }

    #endregion

}

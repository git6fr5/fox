/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    public static float m_ClimbBufferDuration = 0.15f;

    [SerializeField, ReadOnly] private KeyCode m_JumpKey = KeyCode.Space;

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


    protected override void GetInput() {
        m_MoveInput = Input.GetAxisRaw("Horizontal");
        m_JumpInput = Input.GetKeyDown(m_JumpKey);
        m_FloatInput = Input.GetKeyDown(m_JumpKey) ? true : (Input.GetKeyUp(m_JumpKey) ? false : m_FloatInput);

        m_ClimbInput = Input.GetAxisRaw("Vertical");
        m_CanClimb = m_CanClimb ? CheckFall() : CheckClimb();
        m_WallClimbing = m_CanClimb && (m_WallClimbing ? true : (m_ClimbInput != 0f ? true : false));
        m_WallJump = !m_WallClimbing ? false : (Input.GetKeyDown(m_JumpKey) ? true : false);
        m_WallJumping = AirborneFlag == Airborne.Grounded ? false : (m_WallJump ? true : (Input.GetKeyUp(m_JumpKey) ? false : m_WallJumping));

    }

    protected override void ProcessThink(float deltaTime) {
        if (m_WallClimbing) {
            m_WallClimbTicks += deltaTime;
        }
        if (m_CanClimb) {
            m_ClimbBufferTicks += deltaTime;
        }
    }

    private bool CheckClimb() {
        Vector3 direction = DirectionFlag == Direction.Right ? Vector3.right : Vector3.left;
        Collider2D ground = Physics2D.OverlapCircle(transform.position + direction * m_Width, GameRules.MovementPrecision, GameRules.GroundCollisionLayer);
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

    protected override void ProcessMovement(float deltaTime) {
        if (m_WallClimbing && !m_WallJumping) {
            float target = m_Speed * m_ClimbInput;
            if (Mathf.Abs(target - m_Body.velocity.y) >= m_Acceleration * deltaTime) {
                float deltaVelocity = Mathf.Sign(target - m_Body.velocity.y) * m_Acceleration * deltaTime;
                m_Body.velocity = new Vector2(0f, m_Body.velocity.y + deltaVelocity);
            }
            else {
                m_Body.velocity = new Vector2(0f, target);
            }
        }
        else if (m_WallJumping) {
            // Do nothing.
        }
        else {
            base.ProcessMovement(deltaTime);
        }
    }

    protected override void ProcessJump() {
        if (m_WallClimbing) {            
            m_Body.gravityScale = 0f; // Don't fall.
            if (m_WallJump) {
                print("Wall Jump");
                float wallJumpDirection = m_ClimbDirection;
                if (m_ClimbInput == 0f) {
                    wallJumpDirection *= 3f;
                }
                m_Body.velocity = m_JumpForce * (new Vector2(-wallJumpDirection, 1f)).normalized;
            }
        }
        else {
            base.ProcessJump();
        }
    }

}

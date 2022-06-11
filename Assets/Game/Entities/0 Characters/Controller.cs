/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the actions of a character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Controller : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    // Components.
    [HideInInspector] protected Rigidbody2D m_Body; // Handles physics calculations.
    public Rigidbody2D Body => m_Body;
    public State State => m_State;

    [SerializeField] private Input m_Input;
    [SerializeField] private State m_State;
    
    // Knockback.
    [SerializeField, ReadOnly] protected float m_KnockbackTicks;
    public bool m_Knockedback => m_KnockbackTicks > 0f;

    #endregion

    /* --- Unity --- */
    #region Unity 

    private void Start() {
        Init();
    }

    private void Update() {        
        m_Input.Update();
        m_State.Update();
        Run();
    }

    private void Run() {
        if (m_Knockedback) { return; }
    
        Jump();
        Float();
        DoubleJump();
        Dash();
        
        Duck();
        Attack();
    }

    // Runs once every fixed interval.
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        m_State.FixedUpdate(deltaTime);
        Move(deltaTime);
        Knockback(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    protected void Init() {
        m_Body = GetComponent<Rigidbody2D>();
        m_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        m_State.Init(transform, m_Body);
    }

    #endregion

    /* --- Movement --- */
    #region Movement

    protected virtual void Move(float deltaTime) {
        if (m_Knockedback) { return; }
        if (m_State.Dashing) { return; }

        float target = m_State.Speed * m_Input.MoveDirection;
        float difference = Mathf.Abs(target - m_Body.velocity.x);
        float direction = Mathf.Sign(target - m_Body.velocity.x);
        Vector2 deltaVelocity = new Vector2(direction * m_State.Acceleration * deltaTime, 0f);

        if (difference < m_State.Acceleration * deltaTime) {
            m_Body.velocity = new Vector2(target, m_Body.velocity.y);
        }
        else {
            m_Body.velocity += deltaVelocity;
        }

        m_State.SetMoving(m_Input.MoveDirection);
        if (m_Input.MoveDirection != 0f) {
            m_State.SetDirection(m_Input.MoveDirection);
        }
    }

    #endregion

    /* --- Jumping --- */
    #region Jumping

    protected virtual void Jump() {
        if (m_Knockedback) { return; }
        if (m_State.Dashing) { return; }

        if (m_Input.Jump && m_State.CanJump) {
            m_Body.velocity = Vector2.up * m_State.JumpForce;
        }
    }

    private void Float() {
        if (m_State.Dashing) { return; }

        m_Body.gravityScale = m_State.Weight * GameRules.GravityScale;
        if (m_Input.Float && m_State.Rising) {
            m_Body.gravityScale *= m_State.Floatiness;
        }
    }

    private void DoubleJump() {
        if (m_Knockedback) { return; }
        if (m_State.Dashing) { return; }

        if (m_Input.Jump && m_State.CanDoubleJump) {
            m_Body.velocity = Vector2.up * m_State.DoubleJumpForce;
            m_State.UseDoubleJump();
        }
    }

    private void Dash() {
        if (m_Knockedback) { return; }

        if (m_State.CanDash && m_Input.Dash) {
            print("Dashing");
            m_State.StartDash();
            m_Body.velocity = Vector2.zero;
            m_Body.gravityScale = 0f;
            Vector2 direction = m_Input.GetDashDirection(m_State.Direction);
            float duration = m_State.DashDuration;
            StartCoroutine(IEDash(duration, direction));
        }
        
        IEnumerator IEDash(float duration, Vector2 direction) {
            yield return new WaitForSeconds(0.05f);
            m_Body.velocity = direction.normalized * m_State.DashSpeed;
            int precision = 4;
            for (int i = 0; i < precision; i++) {
                Spritesheet.AfterImage(GetComponent<SpriteRenderer>(), transform, duration, 0.5f);
                yield return new WaitForSeconds(duration / (float)precision);
            }
            m_State.EndDash();
            yield return null;
        }

    }

    #endregion

    protected void Duck() {
        if (m_Input.Duck) {
            gameObject.layer = LayerMask.NameToLayer("Ducking");
        }
        else {
            gameObject.layer = LayerMask.NameToLayer("Characters");
        }
    }

    protected void Attack() {
        if (m_Input.Attack) {
            m_State.Projectile.Fire(m_Input.AttackDirection, m_Body.velocity, m_State.Targets);
        }
    }

    public void Hurt(int damage) {
        if (m_Knockedback) { return; }

        m_State.TakeDamage(damage);
        if (m_State.Health <= 0) {
            Kill();
        }

    }

    public void Knockback(Vector2 velocity, float duration) {
        m_Body.velocity = velocity;
        m_KnockbackTicks = duration;
    }

    private void Knockback(float deltaTime) {
        if (m_KnockbackTicks > 0f) {
            m_KnockbackTicks -= deltaTime;
        }
    }

    public void Kill() {
        Destroy(gameObject);
    }

    void OnDrawGizmos() {
        m_State.DebugCollisionChecks(transform.position);
    }


}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///<summary>
public class Enemy : Controller {

    /* --- Variables --- */
    #region Variables
    
    [SerializeField] private float m_PathDuration;
    [SerializeField] private float m_Ticks;
    [SerializeField, ReadOnly] private float m_Direction = 1f;

    [SerializeField] private VisionCone m_VisionCone;
    public bool Aggro => m_VisionCone.Active;
    
    [SerializeField] private Projectile m_Projectile;
    [SerializeField] private float m_AttackTicks;
    [SerializeField] private float m_AttackCooldown;
    
    #endregion

    /* --- Overrides --- */
    #region Overrides
    
    protected override void GetInput() {

        if (!m_VisionCone.Active) {
            if (m_Ticks >= m_PathDuration) {
                m_Direction *= -1f;
                m_Ticks -= m_PathDuration;
            }

            m_MoveInput = m_Direction;
            m_JumpInput = false;
            m_FloatInput = false;
        }
        else {
            float playerDirection = Mathf.Sign((m_VisionCone.player.transform.position - transform.position).x);
            float direction = DirectionFlag == Direction.Right ? 1f : -1f;
            if (direction != playerDirection) {
                m_MoveInput = playerDirection;
            }
            else {
                m_MoveInput = 0f;
            }
            m_JumpInput = false;
            m_FloatInput = false;
            
            Attack();
        }
        
    }

    protected override void ProcessThink(float deltaTime) {
        m_Ticks += deltaTime;
        if (m_VisionCone.Active) {
            m_AttackTicks += deltaTime;
        }
    }

    #endregion

    /* --- Attack --- */
    #region Attack
    
    private void Attack() {

        if (m_AttackTicks >= m_AttackCooldown) {
            m_AttackTicks -= m_AttackCooldown;
            Fire();
        }

    }

    private void Fire() {
        Vector3 playerDirection = (m_VisionCone.player.transform.position - transform.position).normalized;
        m_Projectile.Create(playerDirection);
    }
    
    #endregion

}
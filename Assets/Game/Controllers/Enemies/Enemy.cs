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

    public bool Aggro = false;
    [SerializeField] private float m_AggroBufferInterval = 1f;
    [SerializeField, ReadOnly] private float m_AggroTicks;
    
    #endregion

    /* --- Overrides --- */
    #region Overrides
    
    protected override void GetInput() {

        bool wasAggro = Aggro;
        Aggro = m_VisionCone.Active;

        if (Aggro && !wasAggro) {
            JustSawPlayer();
            return;
        }

        if (!Aggro) {
            IdleBehaviour();
        } 
        else {
            AggroBehaviour();
        }
        
    }

    private void IdleBehaviour() {
        if (m_Ticks >= m_PathDuration) {
            m_Direction *= -1f;
            m_Ticks -= m_PathDuration;
        }

        m_MoveInput = m_Direction;
        m_JumpInput = false;
        m_FloatInput = false;
        m_AttackInput = false;

    }

    private void AggroBehaviour() {
        float playerDirection = Mathf.Sign((m_VisionCone.player.transform.position - transform.position).x);
        float direction = DirectionFlag == Direction.Right ? 1f : -1f;
        if (direction != playerDirection) {
            m_MoveInput = playerDirection;
        }
        else {
            m_MoveInput = 0f;
        }

        m_FloatInput = true;

        m_JumpInput = false;
        // if (m_Projectile.WillThisBeReadyToFireIn(0.2f)) {
        //     m_JumpInput = true;
        // }
        m_AttackInput = m_AggroTicks >= m_AggroBufferInterval ? true : (m_AggroTicks <= 0f ? false : m_AttackInput);

        Vector2 playerPosition = m_VisionCone.player.transform.position;
        Vector2 playerDisplacement = (Vector2)playerPosition - (Vector2)transform.position;
        m_AttackDirection = playerDisplacement.normalized; // + Vector2.up * Mathf.Abs(playerDisplacement.x) / (0.25f * m_Projectile.Speed * m_Projectile.Speed);
    }

    private void JustSawPlayer() {
        m_MoveInput = 0f;
        m_JumpInput = true;
        m_FloatInput = true;
        m_AttackInput = false;
    }

    protected override void ProcessThink(float deltaTime) {
        m_Ticks += deltaTime;

        if (m_VisionCone.Active) {
            m_AggroTicks += deltaTime;
        }
        else if (!m_VisionCone.Active) {
            m_AggroTicks -= deltaTime;
        }
        m_AggroTicks = m_AggroTicks >= m_AggroBufferInterval ? m_AggroBufferInterval : (m_AggroTicks <= 0f ? 0f : m_AggroTicks);

        base.ProcessThink(deltaTime);
    }

    #endregion

}
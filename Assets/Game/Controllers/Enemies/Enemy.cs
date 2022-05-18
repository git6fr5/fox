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
    
    #endregion

    /* --- Overrides --- */
    #region Overrides
    
    protected override void GetInput() {

        if (!Aggro) {
            IdleBehaviour();
        } else {
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

        m_JumpInput = false;
        m_FloatInput = false;
        m_AttackInput = true;
        m_AttackDirection = ((Vector2)m_VisionCone.player.transform.position - (Vector2)transform.position).normalized;
    }

    protected override void ProcessThink(float deltaTime) {
        m_Ticks += deltaTime;
    }

    #endregion

}
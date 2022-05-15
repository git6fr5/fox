/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class Enemy : Controller {

    [SerializeField] private float m_PathDuration;
    [SerializeField] private float m_Ticks;
    [SerializeField, ReadOnly] private float m_Direction = 1f;

    [SerializeField] private VisionCone m_VisionCone;

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
            m_MoveInput = 0f;
            m_JumpInput = false;
            m_FloatInput = false;
        }
        
    }

    protected override void ProcessThink(float deltaTime) {
        m_Ticks += deltaTime;
    }

}
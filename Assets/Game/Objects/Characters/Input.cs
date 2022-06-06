/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class Input : MonoBehaviour {

    // Controls
    [SerializeField, ReadOnly] protected Vector2 m_Direction;
    public float MoveDirection => m_Direction.x;
    [SerializeField, ReadOnly] protected bool m_Jump;
    public bool Jump => m_Jump;
    [SerializeField, ReadOnly] protected bool m_Attack;
    public bool Attack => m_Attack;
    [SerializeField, ReadOnly] protected bool m_Float;
    public bool Float => m_Float;
    [SerializeField, ReadOnly] protected bool m_Duck;
    public bool Duck => m_Duck;
    [SerializeField, ReadOnly] protected Vector2 m_AttackDirection;
    public Vector2 AttackDirection => m_AttackDirection;

    // Dash.
    [SerializeField, ReadOnly] protected bool m_Dash;
    public bool Dash => m_Dash;

    // Double Jump.
    [SerializeField, ReadOnly] protected bool m_DoubleJump;
    public bool DoubleJump => m_DoubleJump;


    public virtual void Update() {
        
    }

    public Vector2 GetDashDirection(float direction) {
        if (m_Direction != Vector2.zero) {
            return m_Direction;
        }
        return new Vector2(direction, 0f);
    }

}
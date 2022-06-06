/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class PlayerInput : Input {

    [SerializeField, ReadOnly] private KeyCode m_JumpKey = KeyCode.Space;

    public override void Update() {
        GetMove();
        GetJump();
        GetDash();
    }

    public void GetMove() {
        float x = UnityEngine.Input.GetAxisRaw("Horizontal");
        float y = UnityEngine.Input.GetAxisRaw("Vertical");
        m_Direction = new Vector2(x, y);
    }

    public void GetJump() {
        m_Jump = UnityEngine.Input.GetKeyDown(m_JumpKey);
        m_Float = UnityEngine.Input.GetKeyDown(m_JumpKey) ? true : (UnityEngine.Input.GetKeyUp(m_JumpKey) ? false : m_Float);
    }

    public void GetDash() {
        m_Dash = UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetKeyDown(KeyCode.K);
    }

}
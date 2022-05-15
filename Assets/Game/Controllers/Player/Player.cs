/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    [SerializeField, ReadOnly] private KeyCode m_JumpKey = KeyCode.Space;

    protected override void GetInput() {
        m_MoveInput = Input.GetAxisRaw("Horizontal");
        m_JumpInput = Input.GetKeyDown(m_JumpKey);
        m_FloatInput = Input.GetKeyDown(m_JumpKey) ? true : (Input.GetKeyUp(m_JumpKey) ? false : m_FloatInput);
    }

}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class Player : Input {

    [SerializeField, ReadOnly] private KeyCode m_JumpKey = KeyCode.Space;

    [SerializeField, ReadOnly] private Level m_Level;
    public Level level => m_Level;

    [SerializeField, ReadOnly] private Checkpoint m_Checkpoint;
    public Checkpoint checkpoint => m_Checkpoint;

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

    public void SetLevel(Level level) {
        m_Level = level;
    }

    public void SetCheck(Checkpoint checkpoint) {
        m_Checkpoint = checkpoint;
    }

}
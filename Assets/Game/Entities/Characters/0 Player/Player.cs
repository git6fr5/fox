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
        GetAttack();
        GetDuck();
        GetJump();
        GetDash();
        GetClimb();
        GetInteract();
    }

    public void GetMove() {
        float x = UnityEngine.Input.GetAxisRaw("Horizontal");
        float y = UnityEngine.Input.GetAxisRaw("Vertical");
        m_Direction = new Vector2(x, y);
    }

    public void GetDuck() {
        m_Duck = UnityEngine.Input.GetAxisRaw("Vertical") == -1f;
    }

    public void GetJump() {
        m_Jump = UnityEngine.Input.GetKeyDown(m_JumpKey);
        m_Float = UnityEngine.Input.GetKeyDown(m_JumpKey) ? true : (UnityEngine.Input.GetKeyUp(m_JumpKey) ? false : m_Float);
    }

    public void GetAttack() {
        m_Attack = UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.GetKeyDown(KeyCode.J);
        m_AttackDirection = (Screen.MousePosition - (Vector2)transform.position).normalized;
    }

    public void GetDash() {
        m_Dash = UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetKeyDown(KeyCode.K);
    }

    public void GetClimb() {
        m_Climb = UnityEngine.Input.GetAxisRaw("Vertical") != 0f;
    }

    public void GetInteract() {
        float interactRadius = 1f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRadius, GameRules.InteractableCollisionLayer);
        float closestNPCDistance = Mathf.Infinity;

        NPC m_NPC = null;
        for (int i = 0; i < colliders.Length; i++) {
            NPC npc = colliders[i].GetComponent<NPC>();
            float distance = npc == null ? 0f : (npc.transform.position - transform.position).magnitude;
            if (npc != null && distance < closestNPCDistance) {
                m_NPC = npc;
                closestNPCDistance = distance;
            }
        }

        bool m_Interact = UnityEngine.Input.GetKeyDown(KeyCode.E);

        // Move to controller later.
        if (m_Interact && m_NPC != null) {
            m_NPC.Interact();
        }
    }

    public void SetLevel(Level level) {
        m_Level = level;
    }

    public void SetCheck(Checkpoint checkpoint) {
        m_Checkpoint = checkpoint;
    }

}
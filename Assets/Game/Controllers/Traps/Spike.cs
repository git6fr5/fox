/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class Spike : Trap {

    private Vector2 m_Offset = new Vector2(0f, -0.5f);
    private Vector2 m_Size = new Vector2(1f, 0.25f);

    protected override bool Trigger() {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)m_Offset, m_Size, 0f);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].GetComponent<Player>()) {
                print("Hello");
                return true;
            }
        }
        return false;
    }

    protected override void Activate() {
        
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + (Vector3)m_Offset, (Vector3)m_Size);
    }

}
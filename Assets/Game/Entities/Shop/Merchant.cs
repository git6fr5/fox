/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Merchant : NPC {

    public Armory m_Armory;

    /* --- Unity --- */
    #region Unity
    
    public override void Interact() {
        print("Interacting");
        m_Armory.Show();
    }
    
    #endregion

}
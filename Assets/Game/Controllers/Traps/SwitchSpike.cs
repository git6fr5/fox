/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class SwitchSpike : Spike {

    public void SwitchState() {
        Flip();
    }

    public void InitOff() {
        Init();
        Flip();
    }

}
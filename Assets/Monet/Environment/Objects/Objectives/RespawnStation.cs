/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class RespawnStation : Station {

        protected override void Activate(State state) {
            state.SetRespawn(this);
        }
        
    }
}
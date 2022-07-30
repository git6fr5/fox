/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class HealingStation : Station {

        protected override void Activate(State state) {
            state.OnHeal();
        }
        
    }
}
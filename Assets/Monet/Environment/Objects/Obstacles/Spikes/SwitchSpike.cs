/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SwitchSpike : Spike, ISwitch {

        public void Flip() {
            OnFlip();
        }

    }
}
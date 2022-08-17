/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
 using System.Linq;   
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SwitchStation : Station {

        protected override void Activate(State state) {
            var switches = FindObjectsOfType<MonoBehaviour>().OfType<ISwitch>();
            foreach (ISwitch s in switches) {
                s.Flip();
            }
        }

        

    }
}
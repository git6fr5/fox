/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Pillbug : Enemy {

        public override void OnUpdate() {
            PatrolAction();
        }

    }
}
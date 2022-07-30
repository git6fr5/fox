/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// The most basic type of enemy that walks in
    /// between two points while waiting a little bit
    /// at the end of each point of its path.
    ///<summary>
    public class Crawler : Enemy {

        // Runs once every frame to update the input.
        public override void OnUpdate() {
            PatrolAction();
        }

    }
}
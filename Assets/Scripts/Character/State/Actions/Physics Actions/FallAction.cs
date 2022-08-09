/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;

namespace Platformer.Character.Actions {

    ///<summary>
    /// Figures out how the character should fall.
    ///<summary>
    [System.Serializable]
    public class FallAction : PhysicsAction {

        [SerializeField] private float m_AntiGravityBuffer;
        [SerializeField, ReadOnly] private float m_AntiGravityTicks;

        // Process the physics of this action.
        public override void Process(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            // if (state.Disabled) { return; }

            // body.gravityScale = Game.Physics.GravityScale;
            // if (!state.OnGround) { 

            //     if (input.Action0.Held && state.Rising) {
            //         body.gravityScale *= weight;
            //     }
            //     else {
            //         body.gravityScale *= (weight * sink);
            //         if (!rising && antiGravTicks > 0f) {
            //             body.gravityScale *= antiGravFactor;
            //         }
            //     }

            // }
        }

    }
}
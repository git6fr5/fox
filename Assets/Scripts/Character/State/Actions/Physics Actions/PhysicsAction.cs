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
    /// An abstract class defining the functionality of a
    /// character's physics. 
    ///<summary>
    [System.Serializable]
    public abstract class PhysicsAction {

        // Process the physics of this action.
        public abstract void Process(Rigidbody2D body, InputSystem input, CharacterState state, float dt);

    }
}
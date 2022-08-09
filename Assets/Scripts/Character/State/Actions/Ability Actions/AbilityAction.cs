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
    /// character's ability. 
    ///<summary>
    [System.Serializable]
    public abstract class AbilityAction {
        
        // When this ability is activated.
        public abstract void Activate(Rigidbody2D body, InputSystem input, CharacterState state);
        
        // Refreshes the settings for this ability every interval.
        public abstract void Refresh(Rigidbody2D body, CharacterState state, float dt);

        // Process the state and input to decide whether to activate the ability.
        public void Process(Rigidbody2D body, InputSystem input, CharacterState state) {
            bool canActivate = CheckState(state);
            bool doActivate = CheckInput(input);
            if (canActivate && doActivate) {
                Activate(body, input, state);
            }
        }
        
        // Checks the state for whether this ability can be activated.
        public virtual bool CheckState(CharacterState state) {
            return false;
        }

        // Checks the input for whether this ability should be activated.
        public virtual bool CheckInput(InputSystem input) {
            return false;
        }


    }

}
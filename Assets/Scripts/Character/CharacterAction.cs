/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Input;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Actions;

namespace Platformer.Character.Actions {

    ///<summary>
    /// An abstract class defining the functionality of a
    /// character's action. 
    ///<summary>
    [System.Serializable]
    public abstract class CharacterAction {

        #region Fields
        
        /* --- Member Variables --- */
        
        // Checks whether the activation conditions have been fulfilled.
        [SerializeField, ReadOnly] 
        protected bool m_Refreshed;

        // Checks whether this ability is enabled.
        [SerializeField] 
        protected bool m_Enabled;

        #endregion

        #region Methods
    
        // When this ability is activated.
        public abstract void InputUpdate(CharacterController controller);
        
        // Refreshes the settings for this ability every interval.
        public abstract void PhysicsUpdate(CharacterController controller, float dt);

        #endregion

    }

}


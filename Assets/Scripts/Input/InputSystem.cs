/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Input;

namespace Platformer.Input {

    ///<summary>
    /// Collects the inputs for a character.
    ///<summary>
    public abstract class InputSystem : MonoBehaviour {

        #region Fields.

        /* --- Member Variables --- */
        
        // The characters directional input.
        [SerializeField] 
        protected DirectionalInput m_Direction = new DirectionalInput();
        public DirectionalInput Direction => m_Direction;

        // The characters action inputs.
        [SerializeField] 
        protected ActionInput[] m_Actions = null;
        public ActionInput[] Actions => m_Actions;
        
        #endregion

        #region Methods.

        // Runs once every frame.
        void Update() {
            UpdateInputs(Time.deltaTime);
        }

        // Updates the inputs.
        protected virtual void UpdateInputs(float dt) {
            // Updates the directional input.
            m_Direction.OnUpdate(Vector2.zero);

            // Updates each of the action buttons.
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i].OnUpdate(false, false, dt);
            }

        }

        #endregion

    }

}

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character.Input;

namespace Platformer.Character.Input {

    ///<summary>
    /// Collects the inputs for a character.
    ///<summary>
    public abstract class InputSystem : MonoBehaviour {

        /* --- Variables --- */
        #region Variables
        
        // The characters directional input.
        [SerializeField] protected DirectionalInput m_Direction;

        // The characters action inputs.
        [SerializeField] protected ActionInput m_Action0;
        [SerializeField] protected ActionInput m_Action1;
        [SerializeField] protected ActionInput m_Action2;

        // The outputs.
        public DirectionalInput Direction => m_Direction;
        public ActionInput Action0 => m_Action0;
        public ActionInput Action1 => m_Action1;
        public ActionInput Action2 => m_Action2;
        
        #endregion

        void Start() {
            m_Direction = new DirectionalInput();
            m_Action0 = new ActionInput();
            m_Action1 = new ActionInput();
            m_Action2 = new ActionInput();
        }

        void Update() {
            Think(Time.deltaTime);
        }

        // Updates the inputs.
        protected virtual void Think(float dt) {
            // Updates the directional input.
            m_Direction.OnUpdate(Vector2.zero);

            // Updates each of the action buttons.
            m_Action0.OnUpdate(false, false, dt);
            m_Action1.OnUpdate(false, false, dt);
            m_Action2.OnUpdate(false, false, dt);
        }

    }

}

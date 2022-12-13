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
    /// Collects the inputs from a keyboard.
    ///<summary>
    public class KeyboardInputSystem : InputSystem {

        #region Variables

        // Binding actions to a key.
        [SerializeField] 
        private KeyCode[] m_Keybinds = null;

        #endregion

        void Start() {
            m_Actions = new ActionInput[m_Keybinds.Length];
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i] = new ActionInput();
            }
        }

        // Updates the inputs.
        protected override void UpdateInputs(float dt) {
            // Updates the directional input.
            float x = GetAxis("Horizontal");
            float y = GetAxis("Vertical");
            m_Direction.OnUpdate(new Vector2(x, y));

            // Updates each of the action buttons.
            for (int i = 0; i < m_Actions.Length; i++) {
                bool down = CheckKeybindDown(i);
                bool up = CheckKeybindUp(i);
                m_Actions[i].OnUpdate(down, up, dt);
            }

        }

        // Gets whether the key was pressed this frame.
        public bool CheckKeybindDown(int i) {
            return UnityEngine.Input.GetKeyDown(m_Keybinds[i]);
        }

        // Gets whether the key was released this frame.
        public bool CheckKeybindUp(int i) {
            return UnityEngine.Input.GetKeyUp(m_Keybinds[i]);
        }

        public float GetAxis(string axis) {
            return UnityEngine.Input.GetAxisRaw(axis);
        }

    }

}
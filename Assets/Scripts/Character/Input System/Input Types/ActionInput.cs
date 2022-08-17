/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character.Input;

namespace  Platformer.Character.Input{

    ///<summary>
    /// Processes all the input information for this particular action.
    ///<summary>
    [System.Serializable]
    public class ActionInput {
        
        /* --- Variables --- */
        #region Variables

        // Buffers to allow leeway when inputs are pressed and released.
        public static float PressBuffer = 0.05f;
        [SerializeField, ReadOnly] private float m_PressedTicks;
        public static float ReleaseBuffer = 0.075f;
        [SerializeField, ReadOnly] private float m_ReleasedTicks;

        // Swaps the state of this action input.
        [SerializeField, ReadOnly] private bool m_Held;
        [SerializeField, ReadOnly] private float m_HeldTicks;
        
        // The useable information from this action's state.
        public bool Pressed => m_PressedTicks > 0f;
        public bool Held => m_Held;
        public bool Released => m_ReleasedTicks > 0f;
        
        #endregion

        // Updates this action input.
        public void OnUpdate(bool press, bool release, float dt) {
            Swap(ref m_Held, ref m_HeldTicks, press, release, dt);
            Buffer(ref m_PressedTicks, PressBuffer, press, dt);
            Buffer(ref m_ReleasedTicks, ReleaseBuffer, release, dt);
        }

        // Swaps the state of a boolean given two seperate booleans.
        private void Swap(ref bool state, ref float ticks, bool on, bool off, float dt) {
            state = on ? true : off ? false : m_Held;
            ticks = state ? ticks + dt : 0f;
        }

        // Allows for a little buffer time when a input is pressed or released.
        public static void Buffer(ref float ticks, float buffer, bool predicate, float dt) {
            ticks = predicate ? buffer : ticks - dt;
            ticks = ticks < 0f ? 0f : ticks;
        }

        public void ClearPressBuffer() {
            m_PressedTicks = 0f;
        }

        public void ClearReleaseBuffer() {
            m_ReleasedTicks = 0f;
        }

    }
}
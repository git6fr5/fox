/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Input;

namespace Platformer.Input{

    ///<summary>
    /// Processes all the input information for this particular action.
    ///<summary>
    [System.Serializable]
    public class ActionInput {

        #region Enumerations.

        public enum State {
            Press,
            Release
        }

        #endregion
        
        #region Fields.

        /* --- Static Variables --- */

        // The buffer to process pressing this input.
        public const float PRESS_BUFFER = 0.06f;

        // The buffer to process releasing this input.
        public const float RELEASE_BUFFER = 0.08f;

        // The duration that this input has been held for.
        [SerializeField, ReadOnly] 
        private float m_HeldTicks;

        // Checks the time since this input was pressed.
        [SerializeField, ReadOnly] 
        private float m_PressTicks;

        // Checks the time since this input has been released.
        [SerializeField, ReadOnly] 
        private float m_ReleaseTicks;

        // Whether this input is currently held down.
        public bool Held => m_HeldTicks > 0f;
        
        // Whether this input was just pressed.
        public bool Pressed => m_PressTicks > 0f;

        // Whether this input was just released.
        public bool Released => m_ReleaseTicks > 0f;
        
        #endregion

        #region Methods.

        // Updates this action input.
        public void OnUpdate(bool press, bool release, float dt) {
            UpdateState(ref m_HeldTicks, press, release, dt);
            UpdateBuffer(ref m_PressTicks, ActionInput.PRESS_BUFFER, press, dt);
            UpdateBuffer(ref m_ReleaseTicks, ActionInput.RELEASE_BUFFER, release, dt);
        }

        // Updates the state of a boolean given two seperate booleans.
        private static void UpdateState(ref float ticks, bool on, bool off, float dt) {
            ticks = on ? dt : off ? 0f : ticks > 0f ? ticks + dt : 0f;
        }

        // Allows for a little buffer time when a input is pressed or released.
        public static void UpdateBuffer(ref float ticks, float buffer, bool predicate, float dt) {
            ticks = predicate ? buffer : ticks - dt;
            ticks = ticks < 0f ? 0f : ticks;
        }

        public void Clear(State state) {
            if (state == State.Press) {
                m_PressTicks = 0f;
            }
            else if (state == State.Release) {
                m_ReleaseTicks = 0f;
            }
        }

        #endregion

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Input : MonoBehaviour {

        public static float UserHorizontalInput => UnityEngine.Input.GetAxisRaw("Horizontal");
        public static float UserVerticalInput => UnityEngine.Input.GetAxisRaw("Vertical");

        public static bool KeyDown(UnityEngine.KeyCode keyCode) {
            return UnityEngine.Input.GetKeyDown(keyCode);
        }

        public static bool KeyDownBuffer(UnityEngine.KeyCode keyCode, ref float bufferTicks, float bufferDuration, float dt) {
            if (UnityEngine.Input.GetKeyDown(keyCode)) {
                bufferTicks = bufferDuration;
            }
            if (bufferTicks != 0f) {
                bufferTicks -= dt;
                if (bufferTicks < 0f) {
                    bufferTicks = 0f;
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool KeyUp(UnityEngine.KeyCode keyCode) {
            return UnityEngine.Input.GetKeyUp(keyCode);
        }

        public static bool KeyHeld(UnityEngine.KeyCode keyCode, bool held) {
            if (!held && KeyDown(keyCode)) {
                held = true;
            }
            if (held && KeyUp(keyCode)) {
                held = false;
            }
            return held;
        }

        // Controls
        [SerializeField, ReadOnly] protected Vector2 m_Direction;
        public float MoveDirection => m_Direction.x;

        [SerializeField, ReadOnly] protected bool m_Jump;
        public bool Jump => m_Jump;
        [SerializeField] protected float m_JumpBufferDuration;
        [SerializeField, ReadOnly] protected float m_JumpBufferTicks;

        [SerializeField, ReadOnly] protected bool m_HoldJump;
        public bool HoldJump => m_HoldJump;

        public virtual void OnUpdate() {

        }

        public void ResetJump() {
            m_Jump = false;
            m_JumpBufferTicks = 0f;
        }

    }

}

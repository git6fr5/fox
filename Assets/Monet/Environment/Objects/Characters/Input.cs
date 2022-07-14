/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Input : MonoBehaviour {

        // Controls
        [SerializeField, ReadOnly] protected Vector2 m_Direction;
        public float MoveDirection => m_Direction.x != 0f ? Mathf.Sign(m_Direction.x) : 0f;

        [SerializeField, ReadOnly] protected bool m_Jump;
        public bool Jump => m_Jump;
        
        [SerializeField, ReadOnly] protected bool m_HoldJump;
        public bool HoldJump => m_HoldJump;

        [SerializeField, ReadOnly] protected bool m_Attack;
        public bool Attack => m_Attack;

        [SerializeField, ReadOnly] protected Vector2 m_AttackDirection;
        public Vector2 AttackDirection => m_AttackDirection;

        [SerializeField, ReadOnly] protected bool m_Dash;
        public bool Dash => m_Dash;

        [SerializeField, ReadOnly] protected Vector2 m_DashDirection;
        public Vector2 DashDirection => m_DashDirection;

        public virtual void OnUpdate() {

        }

        public static float UserHorizontalInput => UnityEngine.Input.GetAxisRaw("Horizontal");
        public static float UserVerticalInput => UnityEngine.Input.GetAxisRaw("Vertical");

        public static float AxisBuffer(float currAxis, float cachedAxis, ref float bufferTicks, float bufferDuration, float dt) {
            if (currAxis == cachedAxis) {
                bufferTicks = bufferDuration;
            }
            else {
                bufferTicks -= dt;
                if (bufferTicks < 0f) {
                    bufferTicks = 0f;
                    return currAxis;
                }
            }
            return cachedAxis;
        }

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

        public virtual void ResetAttack() {
            m_Attack = false;
        }

        public virtual void ResetJump() {
            m_Jump = false;
        }

        public virtual void ResetDash() {
            m_Dash = false;
        }

    }

}

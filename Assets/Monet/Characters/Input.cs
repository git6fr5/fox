/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public abstract class Input : MonoBehaviour {

        // Runs once every frame to update the input.
        public abstract void OnUpdate();

        #region Controls

        public static float UserHorizontalInput => UnityEngine.Input.GetAxisRaw("Horizontal");
        public static float UserVerticalInput => UnityEngine.Input.GetAxisRaw("Vertical");

        [SerializeField, ReadOnly] protected Vector2 m_Direction;
        public Vector2 FlyDirection => m_Direction.normalized;
        public float MoveDirection => m_Direction.x != 0f ? Mathf.Sign(m_Direction.x) : 0f;

        [SerializeField, ReadOnly] protected bool m_Jump;
        public bool Jump => m_Jump;
        
        [SerializeField, ReadOnly] protected bool m_HoldJump;
        public bool HoldJump => m_HoldJump;

        [SerializeField, ReadOnly] protected bool m_JumpRelease;
        public bool JumpRelease => m_JumpRelease;

        [SerializeField, ReadOnly] protected bool m_Attack;
        public bool Attack => m_Attack;

        [SerializeField, ReadOnly] protected Vector2 m_AttackDirection;
        public Vector2 AttackDirection => m_AttackDirection;

        [SerializeField, ReadOnly] protected bool m_Dash;
        public bool Dash => m_Dash;

        [SerializeField, ReadOnly] protected bool m_HoldDash;
        public bool HoldDash => m_HoldDash;

        [SerializeField, ReadOnly] protected bool m_DashRelease;
        public bool DashRelease => m_DashRelease;

        [SerializeField, ReadOnly] protected Vector2 m_DashDirection;
        public Vector2 DashDirection => m_DashDirection;

        [SerializeField, ReadOnly] protected float m_FacingDirection = 1f;
        public float FacingDirection => m_FacingDirection;

        [SerializeField, ReadOnly] protected bool m_Block;
        public bool Block => m_Block;

        [SerializeField, ReadOnly] protected bool m_BlockRelease;
        public bool BlockRelease => m_BlockRelease;
        
        #endregion

        #region Directional Inputs

        // Buffers inputs along the axis.
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

        public static Vector2 GetLatestDirection(float x, float y, Vector2 curr, ref Vector2 cache) {
            if (x != cache.x) {
                cache = new Vector2(x, y);
                return new Vector2(x, 0f);
            }
            else if (y != cache.y) {
                cache = new Vector2(x, y);
                return new Vector2(0f, y);
            }
            return curr;
        }

        // Gets the direction based on the inputs.
        public static Vector2 GetDirection(float x, float y, Vector2 direction) {
            if (x == 0f && y == 0f) {
                if (direction.x == 0f) {
                    direction.x = 1f;
                }
                direction.y = 0f;
            }
            else {
                direction = new Vector2(x, y);
            }
            return direction;
        }

        // Gets the direction based on the inputs.
        public static Vector2 GetDirection(float x, float y, float facing) {
            Vector2 direction = new Vector2(x, y);
            if (x == 0f && y == 0f) {
                direction.x = facing;
            }
            return direction;
        }

        #endregion

        #region Key Inputs

        // Gets whether a key was just pressed.
        public static bool KeyDown(UnityEngine.KeyCode keyCode) {
            return UnityEngine.Input.GetKeyDown(keyCode);
        }

        // Gets whether a key was just released.
        public static bool KeyUp(UnityEngine.KeyCode keyCode) {
            return UnityEngine.Input.GetKeyUp(keyCode);
        }

        // Allows for a little time buffer when a key is pressed.
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

        // Allows for a little time buffer when a key is pressed.
        public static bool KeyUpBuffer(UnityEngine.KeyCode keyCode, ref float bufferTicks, float bufferDuration, float dt) {
            if (UnityEngine.Input.GetKeyUp(keyCode)) {
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

        // Gets whether a key is held.
        public static bool KeyHeld(UnityEngine.KeyCode keyCode, bool held) {
            if (!held && KeyDown(keyCode)) {
                held = true;
            }
            if (held && KeyUp(keyCode)) {
                held = false;
            }
            return held;
        }

        #endregion

        #region Reset Inputs

        public virtual void ResetAttack() {
            m_Attack = false;
        }

        public virtual void ResetJump() {
            m_Jump = false;
        }

        public virtual void ResetDash() {
            m_Dash = false;
        }

        public virtual void ResetJumpRelease() {
            m_JumpRelease = false;
        }

        public virtual void ResetDashRelease() {
            m_DashRelease = false;
        }

        public virtual void ResetBlockRelease() {
            m_BlockRelease = false;
        }

        #endregion

    }

}

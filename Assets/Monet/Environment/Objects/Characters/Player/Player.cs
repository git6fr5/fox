/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// Handles all the player input logic to control the character.
    ///<summary>
    public class Player : Input {

        #region Variables

        [SerializeField] protected float DownBufferDuration = 0.05f;
        [SerializeField] protected float UpBufferDuration = 0.1f;

        // Jumping.
        [SerializeField] private UnityEngine.KeyCode m_JumpKey = UnityEngine.KeyCode.Space;
        [SerializeField, ReadOnly] protected float m_JumpBufferTicks;
        [SerializeField, ReadOnly] protected float m_JumpReleaseBufferTicks;

        // Attacking.
        [SerializeField] private UnityEngine.KeyCode m_AttackKey = UnityEngine.KeyCode.J;
        // [SerializeField, ReadOnly] protected float m_AttackBufferTicks;

        // Dashing.
        [SerializeField] private UnityEngine.KeyCode m_DashKey = UnityEngine.KeyCode.K;
        [SerializeField, ReadOnly] protected float m_DashBufferTicks;
        [SerializeField, ReadOnly] protected float m_DashReleaseBufferTicks;

        [SerializeField, ReadOnly] protected float m_BlockReleaseBufferTicks;

        // Minimap.
        [HideInInspector] private UnityEngine.KeyCode m_MinimapKey = UnityEngine.KeyCode.M;
        [SerializeField] private Minimap m_Minimap;
        public Minimap CurrentMinimap => m_Minimap;
        [SerializeField, ReadOnly] private bool m_MinimapToggle;

        // Interacting.
        [HideInInspector] private UnityEngine.KeyCode m_InteractKey = UnityEngine.KeyCode.E;
        [SerializeField] private float m_InteractRadius;
        [SerializeField, ReadOnly] private bool m_Interact;
        [SerializeField, ReadOnly] private bool m_Interacting;
        [SerializeField, ReadOnly] private NPC m_ActiveNPC;
        public bool ActiveNPC => m_ActiveNPC;
        [HideInInspector] private UnityEngine.KeyCode m_SelectKey = UnityEngine.KeyCode.Space;
        [SerializeField, ReadOnly] private bool m_Select;
        public bool Select => m_Select;
        [SerializeField, ReadOnly] private bool m_HoldSelect;
        public bool HoldSelect => m_HoldSelect;

        [HideInInspector] private Vector2 m_CachedDirection;

        #endregion

        // Runs once every frame to update the input.
        public override void OnUpdate() {
            if (m_Interacting) { 
                m_Direction = new Vector2(0f, 0f);
                m_Jump = false;
                m_HoldJump = false;
                m_Dash = false;
                m_Select = KeyDown(m_SelectKey);
                m_HoldSelect = KeyHeld(m_SelectKey, m_HoldSelect);
                return; 
            }

            m_Direction = new Vector2(Input.UserHorizontalInput, Input.UserVerticalInput);
            m_FacingDirection = Input.UserHorizontalInput != 0f ?  Input.UserHorizontalInput : m_FacingDirection;

            m_Jump = KeyDownBuffer(m_JumpKey, ref m_JumpBufferTicks, DownBufferDuration, Time.deltaTime);
            m_HoldJump = KeyHeld(m_JumpKey, m_HoldJump);
            m_JumpRelease = KeyUpBuffer(m_JumpKey, ref m_JumpReleaseBufferTicks, UpBufferDuration, Time.deltaTime);

            // m_Dash = KeyDownBuffer(m_DashKey, ref m_DashBufferTicks, DownBufferDuration, Time.deltaTime);
            m_HoldDash = KeyHeld(m_DashKey, m_HoldDash);
            m_DashRelease = KeyUpBuffer(m_DashKey, ref m_DashReleaseBufferTicks, UpBufferDuration, Time.deltaTime);
            m_Dash = m_DashRelease;
            m_DashDirection = GetDirection(Input.UserHorizontalInput, Input.UserVerticalInput, m_FacingDirection);

            m_Attack = KeyHeld(m_AttackKey, m_Attack);
            m_AttackDirection = GetDirection(Input.UserHorizontalInput, Input.UserVerticalInput, m_FacingDirection);

            m_Block = m_Direction.y == -1f;
            Timer.CountdownTicks(ref m_BlockReleaseBufferTicks, m_Block, UpBufferDuration, Time.deltaTime);
            m_BlockRelease = !m_Block && m_BlockReleaseBufferTicks != 0f;
            
            m_MinimapToggle = KeyDown(m_MinimapKey);
            if (m_MinimapToggle) {
                m_Minimap.gameObject.SetActive(!m_Minimap.gameObject.activeSelf);
            }

            m_ActiveNPC = PhysicsCheck.Closest<NPC>(transform.position, m_InteractRadius, Game.Physics.CollisionLayers.Characters);
            m_Interact =  KeyDown(m_InteractKey);
            if (m_ActiveNPC != null && m_Interact) {
                m_ActiveNPC.Interact();
            }

        }

        public void SetInteracting(bool interacting) {
            m_Interacting = interacting;
            Character character = GetComponent<Character>();
            character.Body.constraints = interacting ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
        }

        #region Input Resets.

        public override void ResetJump() {
            base.ResetJump();
            m_JumpBufferTicks = 0f;
        }

        public override void ResetDash() {
            base.ResetDash();
            m_DashBufferTicks = 0f;
        }

        public override void ResetJumpRelease() {
            base.ResetJumpRelease();
            m_JumpReleaseBufferTicks = 0f;
        }

        public override void ResetDashRelease() {
            base.ResetDashRelease();
            m_DashReleaseBufferTicks = 0f;
        }

        public override void ResetBlockRelease() {
            base.ResetBlockRelease();
            m_BlockReleaseBufferTicks = 0f;
        }

        #endregion

    }
}
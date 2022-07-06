/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Player : Input {

        [SerializeField] private UnityEngine.KeyCode m_JumpKey = UnityEngine.KeyCode.Space;
        [SerializeField] protected float m_JumpBufferDuration;
        [SerializeField, ReadOnly] protected float m_JumpBufferTicks;

        [SerializeField] private UnityEngine.KeyCode m_AttackKey = UnityEngine.KeyCode.J;
        [SerializeField] protected float m_AttackBufferDuration;
        [SerializeField, ReadOnly] protected float m_AttackBufferTicks;

        [HideInInspector] private UnityEngine.KeyCode m_DashKey = UnityEngine.KeyCode.K;
        [SerializeField] protected float m_DashBufferDuration;
        [SerializeField, ReadOnly] protected float m_DashBufferTicks;

        [HideInInspector] private UnityEngine.KeyCode m_MinimapKey = UnityEngine.KeyCode.M;
        [SerializeField] private Minimap m_Minimap;
        public Minimap CurrentMinimap => m_Minimap;
        [SerializeField, ReadOnly] private bool m_MinimapToggle;

        [HideInInspector] private UnityEngine.KeyCode m_InteractKey = UnityEngine.KeyCode.E;
        [SerializeField] private float m_InteractRadius;
        [SerializeField, ReadOnly] private bool m_Interact;
        [SerializeField, ReadOnly] private bool m_Interacting;
        [SerializeField, ReadOnly] private NPC m_ActiveNPC;
        public bool ActiveNPC => m_ActiveNPC;
        [HideInInspector] private UnityEngine.KeyCode m_SelectKey = UnityEngine.KeyCode.Space;
        [SerializeField] private bool m_Select;
        public bool Select => m_Select;
        [SerializeField] private bool m_HoldSelect;
        public bool HoldSelect => m_HoldSelect;

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
            m_Jump = KeyDownBuffer(m_JumpKey, ref m_JumpBufferTicks, m_JumpBufferDuration, Time.deltaTime);
            m_HoldJump = KeyHeld(m_JumpKey, m_HoldJump);
            
            m_Dash = KeyDownBuffer(m_DashKey, ref m_DashBufferTicks, m_DashBufferDuration, Time.deltaTime);
            m_DashDirection = GetDirection(Input.UserHorizontalInput, Input.UserVerticalInput, m_DashDirection);

            m_Attack = KeyDownBuffer(m_AttackKey, ref m_AttackBufferTicks, m_AttackBufferDuration, Time.deltaTime);
            m_AttackDirection = GetDirection(Input.UserHorizontalInput, Input.UserVerticalInput, m_AttackDirection);
            // m_AttackDirection = GetDirection(Input.UserHorizontalInput, 0f, m_AttackDirection);
            
            m_MinimapToggle = KeyDown(m_MinimapKey);
            if (m_MinimapToggle) {
                m_Minimap.gameObject.SetActive(!m_Minimap.gameObject.activeSelf);
            }

            m_ActiveNPC = GetActiveNPC();
            m_Interact =  KeyDown(m_InteractKey);
            if (m_ActiveNPC != null && m_Interact) {
                m_ActiveNPC.Interact();
            }

        }

        private Vector2 GetDirection(float x, float y, Vector2 direction) {
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

        private NPC GetActiveNPC() {
            float minDistance = Mathf.Infinity;
            NPC closestNPC = null;
            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(transform.position, m_InteractRadius, Game.Physics.CollisionLayers.Characters);
            for (int i = 0; i < colliders.Length; i++) {
                NPC npc = colliders[i].GetComponent<NPC>();
                if (npc != null) {
                    float distance = (npc.transform.position - transform.position).magnitude;
                    if (distance < minDistance) {
                        closestNPC = npc;
                    }
                }
            }
            return closestNPC;
        }

        public void SetInteracting(bool interacting) {
            m_Interacting = interacting;
            if (interacting) {
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else {
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        public override void ResetJump() {
            base.ResetJump();
            m_JumpBufferTicks = 0f;
        }

        public override void ResetDash() {
            base.ResetDash();
            m_DashBufferTicks = 0f;
        }


    }
}
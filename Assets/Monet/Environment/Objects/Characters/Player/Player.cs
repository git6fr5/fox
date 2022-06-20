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

        [SerializeField] private Minimap m_Minimap;
        public Minimap CurrentMinimap => m_Minimap;
        [SerializeField] private bool m_MinimapToggle;

        public override void OnUpdate() {
            m_Direction = new Vector2(Input.UserHorizontalInput, Input.UserVerticalInput);
            m_Jump = KeyDownBuffer(UnityEngine.KeyCode.Space, ref m_JumpBufferTicks, m_JumpBufferDuration, Time.deltaTime);
            m_HoldJump = KeyHeld(UnityEngine.KeyCode.Space, m_HoldJump);

            m_MinimapToggle = KeyDown(KeyCode.E);
            if (m_MinimapToggle) {
                m_Minimap.gameObject.SetActive(!m_Minimap.gameObject.activeSelf);
            }

        }

    }
}
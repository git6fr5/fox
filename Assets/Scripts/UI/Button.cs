/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    public class Button : MonoBehaviour {

        [SerializeField, ReadOnly] private bool m_Pressed;
        [SerializeField, ReadOnly] private float m_PressTicks = 0f;
        private static float PressBuffer = 0.075f;

        void Update() {
            if (m_Pressed) {
                m_PressTicks += Time.deltaTime;
                if (m_PressTicks >= PressBuffer) {
                    m_PressTicks = PressBuffer;
                    Activate();
                }
            }
            else {
                m_PressTicks -= Time.deltaTime;
                if (m_PressTicks <= 0f) {
                    m_PressTicks = 0f;
                }
            }
            
            foreach (Transform child in transform) {
                if (child.GetComponent<Image>() != null) {
                    Color color = Color.white * (1f - 0.5f * m_PressTicks / PressBuffer);
                    color.a = 1f;
                    child.GetComponent<Image>().color = color;
                }
            }
        }

        void OnMouseDown() {
            m_Pressed = true;
            SoundManager.PlaySound(UI.MainUI.ClickSound);
        }

        public virtual void Activate() {

        }

    }
}
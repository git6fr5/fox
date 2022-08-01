/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monet;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    public class Title : MonoBehaviour {

        [SerializeField] private Text m_Textbox;
        [SerializeField, ReadOnly] private Color m_TextColor;

        [SerializeField, ReadOnly] private float m_FadeTicks;

        [SerializeField] private float m_WaitTime;

        [SerializeField, ReadOnly] private bool m_FadingIn;
        [SerializeField] private float m_FadeInDuration;

        [SerializeField] private float m_HoldTime;

        [SerializeField, ReadOnly] private bool m_FadingOut;
        [SerializeField] private float m_FadeOutDuration;

        [HideInInspector] private float m_Ratio;
        [HideInInspector] private Vector3 m_Origin;

        void Start() {
            m_TextColor = m_Textbox.color;
            StartCoroutine(IEPlay());
            m_TextColor.a = 0f;
            m_Textbox.color = m_TextColor;
            m_FadeTicks = 0f;
            m_Ratio = 0f;
            m_Origin = transform.localPosition;
        }

        void FixedUpdate() {
            Fade(Time.fixedDeltaTime);
        }

        private void Fade(float dt) {
            if (m_FadingIn) {
                Timer.TickUp(ref m_FadeTicks, m_FadeInDuration, dt);
                m_Ratio = m_FadeTicks / m_FadeInDuration;
                 
            }
            else if (m_FadingOut) {
                Timer.TickDown(ref m_FadeTicks, dt);
                m_Ratio = m_FadeTicks / m_FadeOutDuration;
                transform.localPosition = m_Origin + (1f-m_Ratio) * Vector3.down; 
            }
            m_TextColor.a = m_Ratio;
            m_Textbox.color = m_TextColor;
        }

        private IEnumerator IEPlay() {
            yield return new WaitForSeconds(m_WaitTime);
            m_FadeTicks = 0f;
            m_FadingIn = true;

            yield return new WaitUntil(() => m_FadeTicks == m_FadeInDuration);
            m_FadingIn = false;
            
            yield return new WaitForSeconds(m_HoldTime);
            m_FadeTicks = m_FadeOutDuration;
            m_FadingOut = true;

            yield return new WaitUntil(() => m_FadeTicks == 0f);
            m_FadingOut = false;
            gameObject.SetActive(false);

            yield return null;
        }

        

    }
    
}
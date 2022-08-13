/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    /// <summary>
    /// Plays a particle animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Particle : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // Components.
        private SpriteRenderer m_SpriteRenderer;

        // Sprites.
        [SerializeField] private Sprite[] m_Sprites;

        // Settings.
        [SerializeField] private bool m_Loop;

        // Animation.
        [SerializeField] private int m_FrameRate = 12;
        [SerializeField, ReadOnly] private int m_Frame;
        [SerializeField, ReadOnly] private float m_Ticks;

        [SerializeField] private bool m_Pause = false;
        [SerializeField] private float m_PauseDuration = 0f;
        [SerializeField, ReadOnly] private float m_PauseTicks = 0f;

        #endregion

        void Start() {
            Init();
        }

        private void Update() {
            Timer.CountdownTicks(ref m_PauseTicks, false, m_PauseDuration, Time.deltaTime);
            if (m_PauseTicks == 0f) {
                Animate(Time.deltaTime);
            }
        }

        public void Init() {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Animate(float deltaTime) {
            // Set the current frame.
            m_Frame = (int)Mathf.Floor(m_Ticks * m_FrameRate);
            if (!m_Loop && m_Frame >= m_Sprites.Length) {
                Destroy(gameObject);
            }
            else if (m_Pause && m_Frame >= m_Sprites.Length) {
                Timer.CountdownTicks(ref m_PauseTicks, true, m_PauseDuration, 0f);
                m_SpriteRenderer.sprite = m_Sprites[0];
                m_Ticks = 0f;
            }
            m_Frame = m_Frame % m_Sprites.Length;
            m_SpriteRenderer.sprite = m_Sprites[m_Frame];

            m_Ticks += deltaTime;

        }
        
    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using Platformer;
using Platformer.Utilites;
using Platformer.Obstacles;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class AlternatingPlatform : Platform {

        public enum Beat {
            A, B
        }

        [SerializeField] private Beat m_Beat;
        private static float Period = 4f;

        [SerializeField] private GameObject m_DisabledObject;

        [SerializeField] private bool m_PreChange = true;
        [SerializeField] private AudioClip m_PreChangeSound;
        [SerializeField] private AudioClip m_ChangeSound;

        public override void Init(int length, Vector3[] path) {
            base.Init(length, path);
            Obstacle.EditSpline(m_DisabledObject.GetComponent<SpriteShapeController>().spline, length);
        }

        void FixedUpdate() {
            
            float t = Game.Ticks % Period;
            bool enableA = t < Period / 2f;
            bool change = (Game.Ticks % (Period * 0.5f)) > 0.5f * Period - 0.5f;

            if (!m_PreChange && change && m_Hitbox.enabled) {
                SoundManager.PlaySound(m_PreChangeSound, 0.05f);
            }
            m_PreChange = change;

            Color color = Screen.ForegroundColorShift;
            if (m_Beat == Beat.A) {
                // color *= enableA ? 1f : 0.25f;
                // m_SpriteShapeRenderer.color = color;
                if (enableA != m_Hitbox.enabled && enableA) {
                    SoundManager.PlaySound(m_ChangeSound, 0.03f);
                }

                m_SpriteShapeRenderer.enabled = enableA;
                m_Hitbox.enabled = enableA;
                m_DisabledObject.SetActive(!enableA);
            }

            if (m_Beat == Beat.B) {
                // color *= !enableA ? 1f : 0.25f;
                // m_SpriteShapeRenderer.color = color;
                if (!enableA != m_Hitbox.enabled && !enableA) {
                    SoundManager.PlaySound(m_ChangeSound, 0.03f);
                }

                m_SpriteShapeRenderer.enabled = !enableA;
                m_Hitbox.enabled = !enableA;
                m_DisabledObject.SetActive(enableA);
            }

            if (change) {
                transform.position = m_Origin - Vector3.down * 2f/16f;
            }
            else {
                transform.position = m_Origin;
            }
            
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using Platformer.Utilites;
using Platformer.Obstacles;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    public class CrumblingPlatform : Platform {

        [SerializeField] private bool m_Crumbling;
        [SerializeField] private float m_CrumbleBuffer;
        [SerializeField, ReadOnly] private float m_CrumbleTicks;

        [SerializeField] private float m_ShakeStrength;
        private float Strength => m_ShakeStrength * m_CrumbleTicks / m_CrumbleBuffer;

        [SerializeField] private AudioClip m_CrumblingSound;
        [SerializeField] private AudioClip m_CrumbleSound;

        void LateUpdate() {
            m_Crumbling = m_PressedDown ? true : m_Crumbling;
            Obstacle.Shake(transform, m_Origin, Strength);
        }

        void FixedUpdate() {
            Timer.TriangleTickDownIf(ref m_CrumbleTicks, m_CrumbleBuffer, Time.fixedDeltaTime, m_Crumbling);

            if (m_Crumbling) {
                SoundManager.PlaySound(m_CrumblingSound, Mathf.Sqrt(m_CrumbleTicks / m_CrumbleBuffer) * 0.1f);
            }

            if (m_CrumbleTicks >= m_CrumbleBuffer) {
                Activate(false);
                m_Crumbling = false;
            }
            else if (m_CrumbleTicks == 0f) {
                Activate(true);
            }
        }

        private void Activate(bool activate) {
            m_Hitbox.enabled = activate;
            m_SpriteShapeRenderer.enabled = activate;
            if (!activate && m_Crumbling) {
                SoundManager.PlaySound(m_CrumbleSound, 0.15f);
            }
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class CrumblingPlatform : Platform {

        [SerializeField] private bool m_Crumbling;
        [SerializeField] private float m_CrumbleBuffer;
        [SerializeField, ReadOnly] private float m_CrumbleTicks;

        [SerializeField] private float m_ShakeStrength;
        private float Strength => m_ShakeStrength * m_CrumbleTicks / m_CrumbleBuffer;

        void LateUpdate() {
            m_Crumbling = m_PressedDown ? true : m_Crumbling;
            Obstacle.Shake(transform, m_Origin, Strength);
        }

        void FixedUpdate() {
            Timer.UpdateTicks(ref m_CrumbleTicks, m_Crumbling, m_CrumbleBuffer, Time.fixedDeltaTime);

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
        }

    }

}
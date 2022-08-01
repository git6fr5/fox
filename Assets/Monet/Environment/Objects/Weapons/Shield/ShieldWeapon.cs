/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(ShieldProjectile))]
    public class ShieldWeapon : Weapon {

        /* --- Variables --- */
        #region Variables

        private ShieldProjectile m_ShieldProjectile => GetComponent<ShieldProjectile>();
        public Vector3 ReturnTarget => m_Character.transform.position + m_LocalOrigin;
 
        #endregion

        protected override void Release() {
            if (Attached) {
                Throw();
                SoundManager.PlaySound(m_FireSound);
            }
            else if (!Attached) {
                m_ShieldProjectile.Return();
                Timer.Reset(ref m_Ticks);
            }
            base.Release();
        }

        protected override void WhileActive() {
            bool finished = Timer.TickDown(ref m_Ticks, Time.fixedDeltaTime);
            if (m_Ticks <= 0f && !m_ShieldProjectile.Returning) {
                m_ShieldProjectile.Return();
            }
        }
        
        protected override void Idle() {
            if (!Attached) { 
                if (m_Ticks <= 0f && !m_ShieldProjectile.Returning) {
                    m_ShieldProjectile.Return();
                }
                return; 
            }

            if (!Charging) {
                Animate(m_Character.CharacterFlipbook);
            }
            else {
                Vector3 position = new Vector3(- m_Direction.x, -m_Direction.y, 0f).normalized;
                transform.localPosition = position * m_Power / m_MaxPower * 0.5f;
            }
        }

        private void Throw() {
            m_Character.CharacterController.Knockback(m_Character.Body, -m_Direction.normalized * 5f * PowerRatio, 0.1f);
            m_ShieldProjectile.Activate(m_Direction, m_Targets);
            Timer.Start(ref m_Ticks, PowerRatio * m_Cooldown);
        }

        public void Catch(Vector2 velocity) {
            m_Character.CharacterController.Knockback(m_Character.Body, velocity.normalized * 5f, 0.1f);
            transform.SetParent(m_Character.transform);
            transform.localPosition = m_LocalOrigin;
            transform.localRotation = Quaternion.identity;
        }

        private void Animate(Flipbook flipbook) {
            float flip = flipbook.transform.eulerAngles.y == 0f ? 1f : -1f;
            int period = flipbook.AnimationLength;
            int frame = flipbook.CurrentFrame;
            Vector2 ellipse = new Vector3(0.5f / 16f, 1f / 16f);
            Vector3 origin = transform.parent.position;
            origin += new Vector3(m_LocalOrigin.x * flip, m_LocalOrigin.y, 0f);
            Obstacle.Cycle(transform, frame + 2, period, origin, ellipse);
        }

    }
}
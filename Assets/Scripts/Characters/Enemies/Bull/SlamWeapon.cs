/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SlamWeapon : Weapon {

        /* --- Variables --- */
        #region Variables

        [SerializeField] SlamProjectile m_SlamProjectile;
 
        #endregion

        protected override void Release() {
            Fire();
            SoundManager.PlaySound(m_FireSound);
            base.Release();
        }

        private void Fire() {

            int count = 3; // 5;
            for (int i = 0; i <= count; i++) {

                Vector2 direction = Quaternion.Euler(0f, 0f, (float)i * 180f / (float)count) * Vector2.right;

                Vector3 position = m_SlamProjectile.transform.position;
                GameObject slamObject = Instantiate(m_SlamProjectile.gameObject, position, Quaternion.identity, null);
                SlamProjectile slamProjectile = slamObject.GetComponent<SlamProjectile>();
                slamProjectile.Activate(direction, m_Targets);

            }

            
        }

    }
}
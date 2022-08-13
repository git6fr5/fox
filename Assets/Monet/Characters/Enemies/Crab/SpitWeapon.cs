/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SpitWeapon : Weapon {

        /* --- Variables --- */
        #region Variables

        [SerializeField] SpitProjectile m_SpitProjectile;
 
        #endregion

        protected override void Release() {
            Fire();
            SoundManager.PlaySound(m_FireSound);
            base.Release();
        }

        private void Fire() {
            Vector3 position = m_SpitProjectile.transform.position;
            GameObject spitObject = Instantiate(m_SpitProjectile.gameObject, position, Quaternion.identity, null);
            SpitProjectile spitProjectile = spitObject.GetComponent<SpitProjectile>();
            spitProjectile.Activate(m_Direction, m_Targets);
        }

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;
using Platformer.Physics;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class LockedSpitter : Spitter {

        protected override void Spit() {
            GameObject spitObject = Instantiate(m_Spitball.gameObject, transform.position, Quaternion.identity, null);
            spitObject.SetActive(true);
            
            Spitball spitball = spitObject.GetComponent<Spitball>();

            Vector3 direction = Game.MainPlayer.transform.position - transform.position;
            direction = direction.normalized;

            spitball.Body.gravityScale = 0f;
            spitball.Body.SetVelocity(direction * m_SpitballSpeed);

        }

    }

}
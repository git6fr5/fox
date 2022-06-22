/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class BodySpike : Spike {

        protected override void FixedUpdate() {
            
        }

        void OnTriggerEnter2D(Collider2D collider) {
            Character character = collider.GetComponent<Character>();
            Player player = collider.GetComponent<Player>();
            if (character != null && player != null) {
                Vector3 direction = player.transform.position - transform.position;
                direction = (Vector3)((Vector2)direction).normalized;
                direction.y += 1f;
                character.Damage(1, direction, m_KnockbackForce);
                m_Hitbox.enabled = false;
                StartCoroutine(IEHitbox());
            }
        }

    }
}
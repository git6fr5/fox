/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class GhostSpike : Spike {

        protected override void ProcessCollision(Collider2D collider) {
            Character character = collider.GetComponent<Character>();
            if (character != null && character.IsPlayer) {
                Vector3 knockbackDirection = Vector3.up;
                bool didDamage = character.Damage(1, Vector2.zero, 0f);
                if (didDamage) {
                    character.CharacterController.Knockback(character.Body, Vector2.up * 1.25f, 1.25f);
                    Shatter();
                }
            }
        }

    }
}
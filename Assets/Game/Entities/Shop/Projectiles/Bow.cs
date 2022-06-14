/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class Bow : Projectile {

    public override void Fire(Vector3 direction, Vector2 referenceVelocity, List<string> targets) {
        if (CanFire) {
            
            Projectile projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
            projectile.Init(Quaternion.Euler(0f, 0f, 45f) * direction, referenceVelocity, targets);

            projectile = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<Projectile>();
            projectile.Init(Quaternion.Euler(0f, 0f, -45f) * direction, referenceVelocity, targets);

        }

        base.Fire(direction, referenceVelocity, targets);
    }


}
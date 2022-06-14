using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Scroll : MonoBehaviour {
    
    public bool Dash = true;
    public bool DoubleJump = true;
    public bool Swim = true;
    public bool Climb = true;

    public VisualEffect effect;

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<Player>()) {
            Activate(collider.GetComponent<Controller>().State);
        }
    }

    public void Activate(State playerState) {

        if (effect != null) { effect.Play(); }

        playerState.DashScroll = Dash;
        playerState.DoubleJumpScroll = DoubleJump;
        playerState.SwimScroll = Swim;
        playerState.ClimbScroll = Climb;

        Destroy(gameObject);

    }

}

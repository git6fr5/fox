/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D collider) {
        Player temp = collider.GetComponent<Player>();
        ProcessCollision(temp);
    }
    
    void OnTriggerExit2D(Collider2D collider) {
        Player temp = collider.GetComponent<Player>();
    }

    void ProcessCollision(Player player) {
        if (player == null) { return; }
        player.SetCheck(this);
    }

    void Update() {

        if (GameRules.MainPlayer != null && GameRules.MainPlayer.checkpoint == this) {
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }
        else {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

    }
    

}

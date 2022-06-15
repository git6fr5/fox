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

    public Transform arms;
    private Vector3 origin;
    private float ticks;

    void Start() {
        origin = transform.position;
        arms.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
    }

    void Update() {
        Spin();
        Bob();
    }

    void Bob() {

        transform.position = origin + 0.25f * Vector3.up * Mathf.Sin(ticks);
        ticks += 0.01f;
    }

    void Spin() {

        arms.localPosition = Vector3.right * 0.15f * Mathf.Cos(ticks) + Vector3.up * 0.15f * Mathf.Sin(ticks);
        arms.eulerAngles -= Vector3.forward * 1f;

    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<Player>()) {
            Activate(collider.GetComponent<Controller>().State);
        }
    }

    public void Activate(State playerState) {

        if (effect != null) { effect.Play(); }

        playerState.DashScroll = Dash ? true : playerState.DashScroll;
        playerState.DoubleJumpScroll = DoubleJump ? true : playerState.DoubleJumpScroll;
        playerState.SwimScroll = Swim ? true : playerState.SwimScroll;
        playerState.ClimbScroll = Climb ? true : playerState.ClimbScroll;

        SoundController.PlaySound(sound, transform.position);

        StaticPlayer.Set(playerState);
        
        Destroy(gameObject);

    }

    public AudioClip sound;

}

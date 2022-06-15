/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class StaticPlayer {

    public static bool Dash = false;
    public static bool DoubleJump = false;
    public static bool Swim = false;
    public static bool Climb = false;

    public static int Gold = 0;
    public static int SecureGold = 0;

    // public void Awake() {
    //     DontDestroyOnLoad(gameObject);
    // }

    public static void Get(State playerState) {
        playerState.DashScroll = Dash;
        playerState.DoubleJumpScroll = DoubleJump; 
        playerState.SwimScroll = Swim;
        playerState.ClimbScroll = Climb;
        playerState.SetGold(Gold, SecureGold);
    }

    public static void Set(State playerState) {
        Dash = playerState.DashScroll;
        DoubleJump = playerState.DoubleJumpScroll;
        Swim = playerState.SwimScroll;
        Climb = playerState.ClimbScroll;
        Gold = playerState.Gold;
        SecureGold = playerState.SecureGold;
    }

}
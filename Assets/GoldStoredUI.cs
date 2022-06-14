using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldStoredUI : MonoBehaviour
{
    
    public Text text;

    void Update() {

        string _ = "Gold: " + GameRules.MainPlayer.GetComponent<Controller>().State.Gold;
        _ += "\nSecure Gold: " + GameRules.MainPlayer.GetComponent<Controller>().State.SecureGold;

        text.text = _;

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldStoredUI : MonoBehaviour
{
    
    public Text text;
    public Image image;

    public int Gold;
    public int SecureGold;

    public float showDuration;

    void Update() {

        int gold = GameRules.MainPlayer.GetComponent<Controller>().State.Gold;
        int secureGold = GameRules.MainPlayer.GetComponent<Controller>().State.SecureGold;

        if (gold != Gold || secureGold != SecureGold) {
            Show(gold, secureGold);
        }

    }

    void FixedUpdate() {
        showDuration -= Time.fixedDeltaTime;
        if (showDuration <= 0f) {
            Hide();
            showDuration = 0f;
        }
    }

    void Show(int gold, int secureGold) {
        Gold = gold;
        SecureGold = secureGold;

        string _ = "Gold: " + Gold;
        _ += "\nSecure Gold: " + SecureGold;

        text.text = _;

        showDuration = 3f;

        image.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

    }

    void Hide() {
        image.gameObject.SetActive(false);
        text.gameObject.SetActive(false);

    }


}

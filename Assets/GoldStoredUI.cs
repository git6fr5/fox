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

    void LateUpdate() {

        int gold = GameRules.MainPlayer.GetComponent<Controller>().State.Gold;
        int secureGold = GameRules.MainPlayer.GetComponent<Controller>().State.SecureGold;

        if (gold != Gold || secureGold != SecureGold) {
            Show(gold, secureGold);
        }

        ForceShow();

    }

    bool forceShow;
    public Minimap minimap;

    void ForceShow() {
        forceShow = minimap.tilemap.gameObject.activeSelf;
        if (forceShow) {
            image.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
        }
    }

    void FixedUpdate() {
        showDuration -= Time.fixedDeltaTime;
        if (showDuration <= 0f && !forceShow) {
            Hide();
            showDuration = 0f;
        }
    }

    void Show(int gold, int secureGold) {
        Gold = gold;
        SecureGold = secureGold;

        string _ = " Gold: " + Gold;
        _ += "\n Secure Gold: " + SecureGold;

        text.text = _;

        showDuration = 3f;

        image.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        StaticPlayer.Set(GameRules.MainPlayer.GetComponent<Controller>().State);

    }

    void Hide() {
        image.gameObject.SetActive(false);
        text.gameObject.SetActive(false);

    }


}

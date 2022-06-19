/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite m_InactiveSprite;
    [SerializeField] private Sprite m_ActiveSprite;

    [SerializeField] private VisualEffect m_ActivateEffect;

    #endregion

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
        if (m_ActivateEffect != null) {
            m_ActivateEffect.Play();
        }
    }

    void Update() {

        if (GameRules.MainPlayer != null && GameRules.MainPlayer.checkpoint == this) {
            m_SpriteRenderer.sprite = m_ActiveSprite;
        }
        else {
            m_SpriteRenderer.sprite = m_InactiveSprite;
        }

    }
    

}

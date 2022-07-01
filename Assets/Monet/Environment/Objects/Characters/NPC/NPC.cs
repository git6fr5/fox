/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;
using Monet.UI;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NPC : MonoBehaviour {

        [SerializeField, ReadOnly] private Sparkle m_SpeechSparkle;
        private bool m_Hover => Game.MainPlayer.ActiveNPC == this;
        private bool m_Active => m_Hover && ChatUI.MainChatUI.PromptObject.activeSelf;
        
        [SerializeField] private Twine m_StartingTwine;

        void Start() {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        void Update() {

            if (m_Hover) {
                FacePlayer();
            }

            if (m_SpeechSparkle == null && m_Active && ChatUI.MainChatUI.ActiveTwine.Reading) {
                m_SpeechSparkle = EffectManager.PlaySparkle(EffectManager.Sparkles.Speech, transform, Vector3.up * 0.5f + Vector3.left * 0.5f);
            }
            else if (m_SpeechSparkle != null && !m_Active && !ChatUI.MainChatUI.ActiveTwine.Reading) {
                print("destroying speech sparkle");
                Destroy(m_SpeechSparkle.gameObject);
            }
            

        }

        private void FacePlayer() {
            float direction = (Game.MainPlayer.transform.position.x - transform.position.x);
            float angle = 0f;
            if (direction < 0f) {
                angle = 180f;
            } 
            transform.eulerAngles = angle * Vector3.up;
        }

        public void Interact() {
            // Move player to interact radius.

            m_StartingTwine.Play();
        }

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    public class ChatUI : UI {

        /* --- Variables --- */
        #region Variables
        
        // Singleton.
        public static ChatUI MainChatUI;

        // The textbox where the prompt is read out to.
        [SerializeField] private Text m_PromptTextUI;
        [SerializeField] private GameObject m_PromptObject;
        public GameObject PromptObject => m_PromptObject;

        // The current active components.
        [HideInInspector] private Twine m_ActiveTwine;
        public Twine ActiveTwine => m_ActiveTwine;
        [SerializeField, ReadOnly] private List<GameObject> m_ActiveResponses;
        [SerializeField, ReadOnly] private int m_ActiveResponseIndex = 0;
        // Sounds.
        [SerializeField] public AudioClip NextLineSound;
        [SerializeField] public AudioClip PrintLetterSoundA;
        [SerializeField] public AudioClip PrintLetterSoundB;
        
        #endregion

        // Runs once on instantiation.
        public override void Awake() {
            base.Awake();
            MainChatUI = this;
            m_PromptTextUI.font = m_Font;
            m_PromptTextUI.fontSize = m_FontSize;
            m_PromptTextUI.color = m_FontColor;
            m_PromptObject.SetActive(false);
        }

        // Runs once every frame.
        void Update() {
            if (!m_PromptObject.activeSelf) {
                return;
            }

            if (m_ActiveTwine != null) {
                m_PromptTextUI.text = m_ActiveTwine.PrintText;
            }
            if (m_ActiveResponses != null && m_ActiveResponses.Count > 0) {
                if (Input.KeyDown(KeyCode.D)) {
                    m_ActiveResponseIndex = (m_ActiveResponseIndex + 1) % m_ActiveResponses.Count;
                    // SoundManager.PlaySound();
                }
                else if (Input.KeyDown(KeyCode.A)) {
                    m_ActiveResponseIndex = (m_ActiveResponseIndex - 1);
                    if (m_ActiveResponseIndex < 0) { m_ActiveResponseIndex = m_ActiveResponses.Count -1; }
                    // SoundManager.PlaySound();
                }
                else if (Game.MainPlayer.Select) {
                    ResponseButton button = m_ActiveResponses[m_ActiveResponseIndex].GetComponent<ResponseButton>();
                    button.Activate();
                    // SoundManager.PlaySound();
                }

                for (int i = 0; i < m_ActiveResponses.Count; i++) {
                    m_ActiveResponses[i].transform.localScale = 0.01f * new Vector3(1f, 1f, 1f);
                    if (i == m_ActiveResponseIndex) {
                        m_ActiveResponses[m_ActiveResponseIndex].transform.localScale = 0.0115f * new Vector3(1f, 1f, 1f);
                    }
                }
            }
        }
        
        // Set the current active prompt
        public static void SetTwine(Twine twine) {
            MainChatUI.m_ActiveTwine = twine;
        }

        // Create a responnse.
        public static void CreateResponseButton(Response response, int optionNumber, int optionCount) {
            // Create the button.
            Vector3 position = UI.PositionOnScreen(optionNumber, optionCount, false);
            position.y = ChatUI.MainChatUI.PromptObject.transform.localPosition.y + 1f;
            GameObject responseObject = UI.CreateClickable(response.Text, position, ResponseButton.DefaultSize);
            
            // Add the text for the response.
            UI.AddImage(responseObject, MainChatUI.m_ButtonBackgroundSprite, ResponseButton.DefaultSize);
            UI.AddText(responseObject, response.Text, ResponseButton.DefaultSize);
            
            // Add the response functionality.
            responseObject.AddComponent<ResponseButton>(); // Just add the component in the inspector and remove this line.
            responseObject.GetComponent<ResponseButton>().SetNextTwine(response.NextTwine, response);
            responseObject.SetActive(true);
            
            // Add this response to the list of currently active responses.
            MainChatUI.m_ActiveResponses.Add(responseObject);
        }

        // Reset the displayed responses.
        public static void Reset() {

            // Itterate through and destroy the response objects.
            List<GameObject> responses = MainChatUI.m_ActiveResponses;
            if (responses != null) {
                for (int i = 0; i < responses.Count; i++) {
                    if (responses[i] != null) {
                        Destroy(responses[i]);
                    }
                }
            }

            // Reset the response index
            MainChatUI.m_ActiveResponseIndex = 0;

            // Re-initialize the active response list.
            MainChatUI.m_ActiveResponses = new List<GameObject>();
        }

    }

}
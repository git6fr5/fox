/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet.UI;

namespace Monet.UI {

    public class Twine : MonoBehaviour {

        [SerializeField] private bool m_PlayOnStart;
        [SerializeField] private Prompt m_Prompt;
        [SerializeField] private Response[] m_Responses;

        // The current text being presented.
        [SerializeField, ReadOnly] private bool m_Reading;
        public bool Reading => m_Reading;
        [SerializeField, ReadOnly] private string m_Text;
        public string PrintText => m_Text;

        public void Play() {
            if (!ChatUI.MainChatUI.PromptObject.activeSelf) {
                ChatUI.MainChatUI.PromptObject.SetActive(true);
            }
            Game.MainPlayer.SetInteracting(true);
            StartCoroutine(IERead());
        }

        // Read the prompt out over time.
        private IEnumerator IERead() {
            // Set the chat to follow this prompt.
            ChatUI.SetTwine(this);
            m_Reading = true;

            // Read through the lines in the prompt.
            for (int i = 0; i < m_Prompt.Lines.Length; i++) {
                
                if (i != 0) {
                    // Wait for a click to start the next line.
                    yield return new WaitUntil(() => Game.MainPlayer.Select);

                }
                yield return new WaitForSeconds(Prompt.PrintInterval);
                
                // Print the line letter by letter.
                m_Text = "";
                for (int j = 0; j < m_Prompt.Lines[i].Length; j++) {
                    m_Text += m_Prompt.Lines[i][j];
                    if (j % 2 == 0) {
                        SoundManager.PlaySound(ChatUI.MainChatUI.PrintLetterSoundA);
                    }
                    else {
                        SoundManager.PlaySound(ChatUI.MainChatUI.PrintLetterSoundB);
                    }
                    float interval = Game.MainPlayer.HoldSelect ? Prompt.PrintInterval / 2f : Prompt.PrintInterval;
                    yield return new WaitForSeconds(interval);

                }

                // Wait a frame to reset the inputs.
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => Game.MainPlayer.Select);

            yield return new WaitForSeconds(Prompt.PrintInterval);

            // Present the responses once this is done reading.
            m_Reading = false;
            PresentResponses();

            yield return null;
        }

        // Present the responses.
        private void PresentResponses() {
            if (m_Responses.Length == 0) {
                StartCoroutine(IEExit());
                return;
            }

            for (int i = 0; i < m_Responses.Length; i++) {
                ChatUI.CreateResponseButton(m_Responses[i], i, m_Responses.Length);
            }
        }

        private IEnumerator IEExit() {
            yield return new WaitUntil(() => Game.MainPlayer.Select);
            Game.MainPlayer.SetInteracting(false);
            ChatUI.MainChatUI.PromptObject.SetActive(false);
            yield return null;
        }

    }

}
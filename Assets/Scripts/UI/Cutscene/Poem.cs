/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Poem : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        [SerializeField] private Text m_Textbox;
        [SerializeField, ReadOnly] private bool m_Finished;
        public bool Finished => m_Finished;
        
        // The poem.
        [SerializeField] private string[] m_Lines;
        [SerializeField] private AudioClip m_Ambience;
        [SerializeField] private AudioClip m_Music;

        // The print speeds.
        [SerializeField] private float m_FirstLineInterval;
        [SerializeField] private float m_LastLineInterval;
        
        [HideInInspector] private float m_Ratio = 1f;
        [SerializeField] private float m_PrintInterval;
        private float PrintIntervalRatio => m_Ratio * m_PrintInterval;
        
        [SerializeField] private float m_NewLineInterval;
        private float NewLineIntervalRatio => m_Ratio * m_NewLineInterval;

        // Sounds.
        [SerializeField] private AudioClip m_NarratorSoundA;
        [SerializeField] private AudioClip m_NarratorSoundB;
        
        #endregion

        /* --- Reading --- */
        #region Reading
        
        // Plays the poem onto the screen.
        public void Play() {
            StartCoroutine(IERead());
            SoundManager.SetAmbience(m_Ambience);
            SoundManager.SetMusic(m_Music);
        }

        // Read the prompt out over time.
        private IEnumerator IERead() {
            m_Textbox.text = "";
            m_Finished = false;

            // Read through the lines.
            for (int i = 0; i < m_Lines.Length; i++) {
                float newLineInterval = i == 0 ? m_FirstLineInterval : m_NewLineInterval;
                if (i != 0) {
                    m_Textbox.text += "\n";
                }
                yield return new WaitForSeconds(newLineInterval);
                
                // Print the line letter by letter.
                for (int j = 0; j < m_Lines[i].Length; j++) {
                    PrintSound(j);
                    m_Textbox.text += m_Lines[i][j];
                    yield return (m_Lines[i][j] == ' ' ? 0 : new WaitForSeconds(m_PrintInterval));

                }
            }

            yield return new WaitForSeconds(m_LastLineInterval);
            m_Finished = true;
            yield return null;
        }

        public void SetRatio(float ratio) {
            m_Ratio = ratio;
        }

        private void PrintSound(int index) {
            if (index % 2 == 0) {
                SoundManager.PlaySound(m_NarratorSoundA);
            }
            else {
                SoundManager.PlaySound(m_NarratorSoundB);
            }
        }

        #endregion

    }
        
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class SoundManager : MonoBehaviour {

        // The background music.
        [SerializeField] private AudioClip m_MSC;
        [SerializeField] private float m_MusicVolume;
        [SerializeField] private AudioClip m_Ambience;
        [SerializeField] private float m_AmbientVolume;
        [SerializeField] private float m_MuteMusic;
        [SerializeField] private float m_MuteGameSounds;

        // The sound effects.
        private static AudioSource MSCPlayer;
        private static List<AudioSource> SFXPlayers;

        void Start() {
            CreatePlayer(m_MSC, "Music Player", 0.25f);
            CreatePlayer(m_Ambience, "Ambience", 0.3f);
            CreateSoundEffectPlayers();
        }

        void Update() {

        }

        private static void CreatePlayer(AudioClip music, string name, float volume) {
            MSCPlayer = new GameObject(name, typeof(AudioSource)).GetComponent<AudioSource>();
            MSCPlayer.transform.SetParent(Camera.main.transform);
            MSCPlayer.transform.position = Vector3.zero;
            MSCPlayer.clip = music;
            MSCPlayer.volume = volume;
            MSCPlayer.loop = true;
            MSCPlayer.Play();
        }

        private static void CreateSoundEffectPlayers() {
            SFXPlayers = new List<AudioSource>();
            for (int i = 0; i < 10; i++) {
                SFXPlayers.Add(new GameObject("SFX Player " + i.ToString(), typeof(AudioSource)).GetComponent<AudioSource>());
                SFXPlayers[i].transform.SetParent(Camera.main.transform);
                SFXPlayers[i].transform.position = Vector3.zero;
            }
        }

        public static void PlaySound(AudioClip audioClip, float volume = 0.45f) {
            if (audioClip == null) { return; }

            for (int i = 0; i < SFXPlayers.Count; i++) {
                if (!SFXPlayers[i].isPlaying) {
                    SFXPlayers[i].clip = audioClip;
                    SFXPlayers[i].volume = volume;
                    SFXPlayers[i].Play();
                    return;
                }
            }

            SFXPlayers.Add(new GameObject("SFX Player " + (SFXPlayers.Count - 1).ToString(), typeof(AudioSource)).GetComponent<AudioSource>());
            SFXPlayers[SFXPlayers.Count - 1].transform.SetParent(Camera.main.transform);
            SFXPlayers[SFXPlayers.Count - 1].transform.position = Vector3.zero;
            SFXPlayers[SFXPlayers.Count - 1].clip = audioClip;
            SFXPlayers[SFXPlayers.Count - 1].volume = volume;
            SFXPlayers[SFXPlayers.Count - 1].Play();
        }

    }
}
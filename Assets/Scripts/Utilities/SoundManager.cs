// TODO: Clean.

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;

namespace Platformer.Utilites {

    ///<summary>
    /// Controls the sound in the game.
    ///<summary>
    public class SoundManager : MonoBehaviour {

        // The background music.
        [SerializeField] private AudioClip m_DefaultMSC;
        [SerializeField] private float m_MusicVolume;
        [SerializeField] private AudioClip m_DefaultAmbience;
        [SerializeField] private float m_AmbientVolume;
        [SerializeField] private float m_MuteMusic;
        [SerializeField] private float m_MuteGameSounds;

        // Environemnt sounds.
        [SerializeField] private AudioClip m_GroundImpactSound;
        public AudioClip GroundImpactSound => m_GroundImpactSound;
        [SerializeField] private AudioClip m_GroundStepSoundA;
        public AudioClip GroundStepSoundA => m_GroundStepSoundA;
        [SerializeField] private AudioClip m_GroundStepSoundB;
        public AudioClip GroundStepSoundB => m_GroundStepSoundB;
        
        // The sound effects.
        private static AudioSource MSCSource;
        private static AudioSource AmbientSource;
        private static List<AudioSource> SFXSources;

        public void OnStart() {
            CreatePlayer(m_DefaultMSC, "Music Source", m_MusicVolume, ref MSCSource);
            CreatePlayer(m_DefaultAmbience, "Ambient Source", m_AmbientVolume, ref AmbientSource);
            CreateSoundEffectPlayers();
        }

        private static void CreatePlayer(AudioClip music, string name, float volume, ref AudioSource source) {
            source = new GameObject(name, typeof(AudioSource)).GetComponent<AudioSource>();
            source.transform.SetParent(Camera.main.transform);
            source.transform.position = Vector3.zero;
            source.clip = music;
            source.volume = volume;
            source.loop = true;
            source.Play();
        }

        private static void CreateSoundEffectPlayers() {
            SFXSources = new List<AudioSource>();
            for (int i = 0; i < 10; i++) {
                SFXSources.Add(new GameObject("SFX Source " + i.ToString(), typeof(AudioSource)).GetComponent<AudioSource>());
                SFXSources[i].transform.SetParent(Camera.main.transform);
                SFXSources[i].transform.position = Vector3.zero;
            }
        }

        public static void SetMusic(AudioClip audioClip) {
            MSCSource.clip = audioClip;
        }

        public static void SetAmbience(AudioClip audioClip) {
            AmbientSource.clip = audioClip;
        }


        public static void PlaySound(AudioClip audioClip, float volume = 0.45f) {
            if (audioClip == null || SFXSources == null) { return; }

            List<AudioSource> playingSFX = SFXSources.FindAll(source => source.clip == audioClip && source.isPlaying && source.time < 0.05f);
            float spread = 0.025f;
            if (playingSFX != null && playingSFX.Count > 1) {
                volume /= (float)playingSFX.Count;
                // spread += (playingSFX.Count - 1f) * 0.1f;
            }
             
            for (int i = 0; i < SFXSources.Count; i++) {
                if (!SFXSources[i].isPlaying) {
                    SFXSources[i].clip = audioClip;
                    SFXSources[i].volume = Mathf.Sqrt(volume);
                    SFXSources[i].pitch = Random.Range(1f - spread, 1f + spread);
                    SFXSources[i].Play();
                    return;
                }
            }

            SFXSources.Add(new GameObject("SFX Player " + (SFXSources.Count - 1).ToString(), typeof(AudioSource)).GetComponent<AudioSource>());
            SFXSources[SFXSources.Count - 1].transform.SetParent(Camera.main.transform);
            SFXSources[SFXSources.Count - 1].transform.position = Vector3.zero;
            SFXSources[SFXSources.Count - 1].clip = audioClip;
            SFXSources[SFXSources.Count - 1].volume = volume;
            SFXSources[SFXSources.Count - 1].Play();
        }

    }
}


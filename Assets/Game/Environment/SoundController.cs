/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class SoundController : MonoBehaviour {

    public AudioClip ambience;
    public AudioClip musicIntro;
    public AudioClip music;

    AudioSource ambiencePlayer;
    AudioSource musicPlayer;
    static List<AudioSource> audioPlayers;

    void Start() {

        ambiencePlayer = new GameObject("ambience player", typeof(AudioSource)).GetComponent<AudioSource>();
        ambiencePlayer.transform.SetParent(Camera.main.transform);
        ambiencePlayer.transform.position = Vector3.zero;
        ambiencePlayer.volume = 0.15f;
        ambiencePlayer.clip = ambience;
        ambiencePlayer.Play();
        ambiencePlayer.loop = true;

        musicPlayer = new GameObject("music player", typeof(AudioSource)).GetComponent<AudioSource>();
        musicPlayer.transform.SetParent(Camera.main.transform);
        musicPlayer.transform.position = Vector3.zero;
        musicPlayer.clip = music;
        ambiencePlayer.volume = 0.25f;
        musicPlayer.Play();
        musicPlayer.loop = true;

        audioPlayers = new List<AudioSource>();
        for (int i = 0; i < 10; i++) {
            audioPlayers.Add(new GameObject("audio player " + i.ToString(), typeof(AudioSource)).GetComponent<AudioSource>());
            audioPlayers[i].transform.SetParent(Camera.main.transform);
            audioPlayers[i].transform.position = Vector3.zero;
            
        }
        
    }

    public static void PlaySound(AudioClip audioClip, Vector2 position, float vol = 0.45f) {

        Vector3 screenPos = Screen.Instance.transform.position;
        if ((position - (Vector2)screenPos).sqrMagnitude > 25f * 25f) {
            return;
        }

        Vector3 pos = new Vector3(position.x, position.y, screenPos.z);

        float dist = (position - (Vector2)screenPos).magnitude;
        float volume = 1f;
        if (dist > 10f) {
            volume = 1f - (dist - 10f) / 15f;
        }
        volume *= vol;

        for (int i = 0; i < audioPlayers.Count; i++)
        {
            if (!audioPlayers[i].isPlaying) {
                audioPlayers[i].clip = audioClip;
                audioPlayers[i].volume = volume;
                audioPlayers[i].Play();
                return;
            }

        }

        audioPlayers.Add(new GameObject("audio player " + (audioPlayers.Count - 1).ToString(), typeof(AudioSource)).GetComponent<AudioSource>());
        audioPlayers[audioPlayers.Count - 1].transform.SetParent(Camera.main.transform);
        audioPlayers[audioPlayers.Count - 1].transform.position = Vector3.zero;
        audioPlayers[audioPlayers.Count - 1].clip = audioClip;
        audioPlayers[audioPlayers.Count - 1].volume = volume;
        audioPlayers[audioPlayers.Count - 1].Play();

    }

}
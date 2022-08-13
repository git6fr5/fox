/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class EffectManager : MonoBehaviour {

        public static EffectManager Instance;

        [System.Serializable]
        public class SparkleData {

            [SerializeField] private Sparkle m_Speech;
            public Sparkle Speech => m_Speech;

            [SerializeField] private Sparkle m_Charm;
            public Sparkle Charm => m_Charm;

        }

        [SerializeField] private SparkleData m_Sparkles;
        public static SparkleData Sparkles => Instance.m_Sparkles;

        void Awake() {
            Instance = this;
        }

        public static Sparkle PlaySparkle(Sparkle sparkle, Transform transform, Vector3 offset, float duration = -1f) {
            GameObject sparkleObject = Instantiate(sparkle.gameObject, transform.position, Quaternion.identity, transform);
            sparkleObject.SetActive(true);
            sparkleObject.transform.localPosition = offset;
            if (duration > 0f) {
                Destroy(sparkleObject, duration);
            }
            return sparkleObject.GetComponent<Sparkle>();
        }

        private static void CreatePlayer(AudioClip music, string name, float volume) {
        
        }

        private static void CreateSoundEffectPlayers() {

        }

    }
}
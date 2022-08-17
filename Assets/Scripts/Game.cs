/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.LevelLoader;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class Game : MonoBehaviour {

        // Singleton.
        public static Game Instance;

        [SerializeField] private float m_TimeScale;
        [SerializeField] private float m_Ticks;
        public static float Ticks => Instance.m_Ticks;

        // Player.
        [SerializeField] private CharacterState m_Player;
        public static CharacterState MainPlayer => Instance.m_Player;

        // PhysicsSettings.
        [SerializeField] private PhysicsSettings m_Physics;
        public static PhysicsSettings Physics => Instance.m_Physics;

        [SerializeField] private LDtkLoader m_LevelLoader;
        public static LDtkLoader LevelLoader => Instance.m_LevelLoader;

        [SerializeField] private SoundManager m_SoundManager;
        public static SoundManager SoundManager => Instance.m_SoundManager;

        [SerializeField] private Grid m_Grid;
        public static Grid MainGrid => Instance.m_Grid;

        // Opening level.
        [SerializeField] private string m_OpeningLevel;

        bool m_RampStop;
        float m_RampIncrement = 1f / 128f;
        float m_Ramp = 0f;

        bool m_HitStop;
        int m_HitFrames = 0;
        int m_StopFrames = 16;

        void Awake() {
            Instance = this;
        }

        void Start() {
            Level.InitializeGroundLayer(m_Grid.transform);
            Level.InitializeWaterLayer(m_Grid.transform);
            m_LevelLoader.Init();
            Screen.Instance.gameObject.SetActive(false);
            StartCoroutine(IELoadOpeningLevel());
        }

        private IEnumerator IELoadOpeningLevel() {
            yield return 0;
            m_LevelLoader.SetLoadPoint(m_OpeningLevel, m_Player.transform);
            Screen.Instance.transform.position = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y + 10f, Screen.Instance.transform.position.z);
            Screen.Instance.gameObject.SetActive(true);
            yield return 0;
            m_SoundManager.OnStart();
            yield return null;
        }

        void Update() {
            Time.timeScale = m_TimeScale;

            if (m_RampStop) {
                m_Ramp += m_RampIncrement;
                if (m_Ramp > 0.5f) {
                    m_TimeScale += m_RampIncrement;
                }
                if (m_TimeScale >= 1f) {
                    m_RampStop = false;
                    m_TimeScale = 1f;
                }
            }

            if (m_HitStop) {
                m_HitFrames += 1;
                if (m_HitFrames >= m_StopFrames) {
                    m_TimeScale = 1f;
                }
            }
            
        }

        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
        }

        public static void Pause() {
            Instance.m_TimeScale = 0f;
        }

        public static void HitStop(int frames = 16) {
            if (Instance.m_RampStop) {
                return;
            }

            Pause();
            Instance.m_HitStop = true;
            Instance.m_HitFrames = 0;
            Instance.m_StopFrames = frames;
        }

        public static void RampStop(int frames = 128) {
            Pause();
            Instance.m_Ramp = 0f;
            Instance.m_RampStop = true;
            Instance.m_RampIncrement = 1f / (float)frames;

            Instance.m_HitStop = false;
            Instance.m_HitFrames = 0;
        }

        public static bool Validate<T>(T[] array) {
            return array != null && array.Length > 0;
        }

        public static bool Validate<T>(List<T> list) {
            return list != null && list.Count > 0;
        }

    }
    
}


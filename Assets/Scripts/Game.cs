// TODO: Clean

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

        #region Variables

        // Singleton.
        public static Game Instance;

        // Ticks.
        [SerializeField] private float m_TimeScale;
        [SerializeField] private float m_Ticks;
        public static float Ticks => Instance.m_Ticks;

        // Player.
        [SerializeField] private CharacterState m_Player;
        public static CharacterState MainPlayer => Instance.m_Player;

        // PhysicsSettings.
        [SerializeField] private PhysicsSettings m_Physics;
        public static PhysicsSettings Physics => Instance.m_Physics;

        // Level loading.
        [SerializeField] private LDtkLoader m_LevelLoader;
        public static LDtkLoader LevelLoader => Instance.m_LevelLoader;

        // Sound manager.
        [SerializeField] private SoundManager m_SoundManager;
        public static SoundManager SoundManager => Instance.m_SoundManager;

        // Grid.
        [SerializeField] private UnityEngine.Grid m_Grid;
        public static UnityEngine.Grid MainGrid => Instance.m_Grid;

        // Opening level.
        [SerializeField] private string m_OpeningLevel;

        // Ramp stop.
        [HideInInspector] private bool m_RampStop;
        [HideInInspector] private float m_RampIncrement = 1f / 128f;
        [HideInInspector] private float m_Ramp = 0f;
        // Hit stop.
        [HideInInspector] private bool m_HitStop;
        [HideInInspector] private int m_HitFrames = 0;
        [HideInInspector] private int m_StopFrames = 16;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        // Runs once before the first frame.
        void Start() {
            Level.InitializeGroundLayer(m_Grid.transform);
            Level.InitializeWaterLayer(m_Grid.transform);
            m_LevelLoader.Init();
            Screen.Instance.gameObject.SetActive(false);
            m_Player.gameObject.SetActive(false);
            StartCoroutine(IELoadOpeningLevel());
        }

        // Load the opening level.
        private IEnumerator IELoadOpeningLevel() {
            yield return 0;
            m_LevelLoader.SetLoadPoint(m_OpeningLevel, m_Player.transform);
            Screen.Instance.transform.position = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y + 10f, Screen.Instance.transform.position.z);
            Screen.Instance.gameObject.SetActive(true);
            yield return 0;
            m_SoundManager.OnStart();
            m_Player.gameObject.SetActive(true);
            yield return null;
        }

        // Runs once every frame.
        void Update() {
            Time.timeScale = m_TimeScale;
            UpdateRamp();
            UpdateHit();
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
        }

        // Pause the game.
        public static void Pause() {
            Instance.m_TimeScale = 0f;
        }

        // Run a hit stop.
        public static void HitStop(int frames = 16) {
            if (Instance.m_RampStop) { return; }
            // Pause the game.
            Pause();
            // Set up the hitstop
            Instance.m_HitStop = true;
            Instance.m_HitFrames = 0;
            Instance.m_StopFrames = frames;
        }

        private void UpdateHit() {
            if (m_HitStop) {
                m_HitFrames += 1;
                if (m_HitFrames >= m_StopFrames) {
                    m_TimeScale = 1f;
                }
            }
        }

        // Run a ramp stop.
        public static void RampStop(int frames = 128) {
            // Pause the game.
            Pause();
            // Set up the rampstop
            Instance.m_Ramp = 0f;
            Instance.m_RampStop = true;
            Instance.m_RampIncrement = 1f / (float)frames;
            // Disable the hitstop.
            Instance.m_HitStop = false;
            Instance.m_HitFrames = 0;
        }

        private void UpdateRamp() {
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
        }

        // Validate an array.
        public static bool Validate<T>(T[] array) {
            return array != null && array.Length > 0;
        }

        // Validate a list.
        public static bool Validate<T>(List<T> list) {
            return list != null && list.Count > 0;
        }

    }
    
}


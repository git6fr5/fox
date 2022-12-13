/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer;
using Platformer.Physics;
using Platformer.Character;
using Platformer.LevelLoader;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer {

    ///<summary>
    /// 
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour {

        #region Fields.

        // Singleton.
        public static GameManager Instance;

        // Player.
        [SerializeField] 
        private CharacterController m_Player;
        public static CharacterController MainPlayer => Instance.m_Player;

        // PhysicsSettings.
        [SerializeField] 
        private PhysicsManager m_Physics;
        public static PhysicsManager Physics => Instance.m_Physics;

        // Level loading.
        [SerializeField] 
        private LDtkLoader m_LevelLoader;
        public static LDtkLoader LevelLoader => Instance.m_LevelLoader;

        // Sound manager.
        // [SerializeField] 
        // private SoundManager m_Sounds;
        // public static SoundManager Sounds => Instance.m_Sounds;

        // Grid.
        [SerializeField] 
        private TilemapManager m_Tilemaps;
        public static TilemapManager Tilemaps => Instance.m_Tilemaps;

        // Entities.
        [SerializeField] 
        private EntityManager m_Entities;
        public static EntityManager Entities => Instance.m_Entities;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        void Update() {
            Time.timeScale = m_Physics.TimeScale;
        }

        // Runs once before the first frame.
        void Start() {
            // Load the managers.
            // m_Sounds.OnGameLoad();
            m_Entities.OnGameLoad();
            m_Tilemaps.OnGameLoad();
            m_LevelLoader.OnGameLoad();
            m_Player.gameObject.SetActive(true);
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
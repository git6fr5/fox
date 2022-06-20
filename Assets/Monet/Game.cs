/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(-1000)]
    public class Game : MonoBehaviour {

        // Singleton.
        public static Game Instance;

         // Player.
        [SerializeField] private Player m_Player;
        public static Player MainPlayer => Instance.m_Player;

        // PhysicsSettings.
        [SerializeField] private PhysicsSettings m_Physics;
        public static PhysicsSettings Physics => Instance.m_Physics;

        [SerializeField] private Environment m_Environment;
        public static Environment Environment => Instance.m_Environment;

        [SerializeField] private LDtkLoader m_LevelLoader;
        public static LDtkLoader LevelLoader => Instance.m_LevelLoader;

        [SerializeField] private Grid m_Grid;
        public static Grid MainGrid => Instance.m_Grid;

        // Opening level.
        [SerializeField] private string m_OpeningLevel;

        void Awake() {
            Instance = this;
        }

        void Start() {
            Level.InitializeGroundLayer(m_Grid.transform);
            Level.InitializeWaterLayer(m_Grid.transform);
            m_Environment.Init();
            m_LevelLoader.Init();
            m_LevelLoader.Open(m_OpeningLevel, m_Player.transform);
        }

    }
    
}


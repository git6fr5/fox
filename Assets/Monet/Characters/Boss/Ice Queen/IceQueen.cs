/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class IceQueen : Input {

        // Components.
        public Rigidbody2D Body => GetComponent<Rigidbody2D>();
        public State CharacterState => GetComponent<Character>().CharacterState;


        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField] private Vector3 m_Origin;

        // Tiara.
        public int HealthLoss => CharacterState.MaxHealth - CharacterState.Health;
        public int HealthRatio => CharacterState.Health / CharacterState.Health;

        [SerializeField] private IceQueenTiara m_Tiara;
        [SerializeField] private int m_AttackCount;
        [SerializeField] private float m_WaitInterval;
        [SerializeField] private float m_WaitIntervalDecreasePerHealthLoss;
        [SerializeField] private float m_WaitIntervalVariation;
        private float MinWaitInterval => m_WaitInterval - HealthLoss * m_WaitIntervalDecreasePerHealthLoss - m_WaitIntervalVariation;
        private float MaxWaitInterval => m_WaitInterval - HealthLoss * m_WaitIntervalDecreasePerHealthLoss + m_WaitIntervalVariation;
        public float WaitInterval => UnityEngine.Random.Range(MinWaitInterval, MaxWaitInterval);
        public float FireRateRatio => 0.15f + 0.85f * HealthRatio;

        public int IcicleCount => (int)Mathf.Floor(((float)HealthLoss) / 2f) + 2;
        public int ExtraAttacks => (int)Mathf.Floor(((float)HealthLoss) / 4f);

        [System.Serializable]
        public class AttackPattern {
            [SerializeField] private int m_Count;
            public int Count => m_Count;

            [SerializeField] private float m_FireInterval;
            public float FireInterval => m_FireInterval;

            [SerializeField] private float m_ReleaseDelay;
            public float ReleaseDelay => m_ReleaseDelay;

            [SerializeField] private IceQueenTiara.IceAttackType m_AttackType;
            public IceQueenTiara.IceAttackType AttackType => m_AttackType;
        }

        [SerializeField] private List<AttackPattern> m_AttackPatterns = new List<AttackPattern>();
        [SerializeField] private int m_AttackPatternIndex;
        public AttackPattern CurrAttackPattern => m_AttackPatterns.Count == 0 ? null : m_AttackPatterns[(int)Mathf.Abs(m_AttackPatternIndex) % m_AttackPatterns.Count];

        [System.Serializable]
        public class MovePattern {
            [SerializeField] private EdgeCollider2D m_Collider;
            public Vector2[] Points => m_Collider.points;
        }

        [SerializeField] private List<MovePattern> m_MovePatterns = new List<MovePattern>();
        [SerializeField] private int m_MovePatternIndex;
        [SerializeField] private int m_PathIndex;
        public MovePattern CurrMovePattern => m_MovePatterns.Count == 0 ? null : m_MovePatterns[(int)Mathf.Abs(m_MovePatternIndex) % m_MovePatterns.Count];
        public Vector2[] Path => CurrMovePattern.Points;
        public Vector2 Target => Path[m_PathIndex] + (Vector2)m_Origin;

        void Start() {
            m_Origin = transform.position;
            m_Direction = Vector2.zero;
            Timer.Start(ref m_Ticks, WaitInterval);
            // CreatePieces();
        }

        // Runs once every frame to update the input.
        public override void OnUpdate() {

        }

        void FixedUpdate() {
            UpdateMovement(Time.fixedDeltaTime);
            UpdateAttack(Time.fixedDeltaTime);
            // UpdatePieces(Time.fixedDeltaTime);
        }

        private void UpdateMovement(float deltaTime) {
            // Calculate these values.
            Vector2 displacement = Target - (Vector2)transform.position;
            float dx = Body.velocity.magnitude * Time.fixedDeltaTime;
            bool arrived = displacement.magnitude < dx;
            print(displacement);
            print(arrived);

            // Cycle the patrol array index if the timer has finished ticking down.
            bool restarting = Utilities.CycleIndexIf(ref m_PathIndex, 1, Path.Length, arrived);
            if (restarting) {
                m_MovePatternIndex = Utilities.RandomIndex(m_MovePatterns.Count);
            }
            m_Direction = displacement.normalized;
        }

        private void UpdateAttack(float deltaTime) {
            if (CurrAttackPattern != null) {
                m_Tiara.SetAttackType(CurrAttackPattern.AttackType);
                // m_Tiara.SetAttackType(IceQueenTiara.IceAttackType.DiagonalIciclesRight);
                m_Tiara.SetDifficulty(IcicleCount);
                m_Tiara.SetReleaseDelay(FireRateRatio * CurrAttackPattern.ReleaseDelay);
                bool finish = Timer.TickDown(ref m_Ticks, deltaTime);
                if (finish) {
                    m_Attack = !m_Attack;
                    m_AttackCount += 1;
                    if (m_AttackCount < 2 * (CurrAttackPattern.Count + ExtraAttacks)) {
                        Timer.Start(ref m_Ticks, FireRateRatio * CurrAttackPattern.FireInterval);
                        if (m_AttackCount % 2 == 0) {
                            m_Tiara.FlipReleaseOrder(!m_Tiara.FlipRelease);
                        }
                    }
                    else {
                        m_Attack = false;
                        Timer.Start(ref m_Ticks, WaitInterval);
                        m_AttackCount = 0;
                        m_AttackPatternIndex = Utilities.RandomIndex(m_AttackPatterns.Count);
                        m_Tiara.FlipReleaseOrder(false);
                    }
                }
            }
        }

    }

}
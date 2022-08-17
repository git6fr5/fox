/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class IceQueenTiara : Weapon {

        /* --- Variables --- */
        #region Variables

        public enum IceAttackType {
            FallingIcicles,
            RadialIcicles,
            DiagonalIciclesLeft,
            DiagonalIciclesRight,
            Shards
        }

        [SerializeField, ReadOnly] private float m_CycleTicks;
        [SerializeField] private float m_Period;
        [SerializeField] private Vector3 m_Ellipse;

        [SerializeField] private float m_PeriodFactor;
        public float Period => m_Period * m_PeriodFactor;

        [SerializeField] private IceQueenIcicle m_Icicle;
        [SerializeField] private IceQueenShard m_Shard;
        [HideInInspector] private Dictionary<IceQueenIcicle, Vector2> m_Icicles = new Dictionary<IceQueenIcicle, Vector2>();
        [HideInInspector] private List<IceQueenShard> m_Shards = new List<IceQueenShard>();

        [SerializeField] private IceAttackType m_AttackType;
        [SerializeField] private float m_ReleaseDelay;

        [SerializeField, Range(1, 12)] private int m_Difficulty;
        [SerializeField] private bool m_FlipRelease;
        public bool FlipRelease => m_FlipRelease;

        public float Down => 180f;
        public float DiagonalLeft => 135f;
        public float DiagonalRight => 225;

        [SerializeField] private float m_Spacing = 1.5f;
        public float Radius => 2.5f;
        public float Spread => (float)m_Difficulty * m_Spacing;
        public int Count => m_Difficulty;

        [SerializeField] private float m_AngleIncrment = 10f;
        public float StartAngle => 180f - m_AngleIncrment * (Count - 1);
        public float Arc => 2f * m_AngleIncrment * (Count - 1); 

        [SerializeField] private float m_InnerRadius;
        [SerializeField] private float m_InnerPeriod;
        [SerializeField] private float m_OuterRadius;
        [SerializeField] private float m_OuterPeriod;

        #endregion

        public void SetAttackType(IceAttackType attackType) {
            m_AttackType = attackType;
        }

        public void SetDifficulty(int difficulty) {
            m_Difficulty = difficulty;
        }

        public void SetReleaseDelay(float delay) {
            m_ReleaseDelay = delay;
        }

        public void FlipReleaseOrder(bool flip) {
            m_FlipRelease = flip;
        }

        protected override void Charge(float deltaTime) {
            if (m_Power == 0f) {
                switch (m_AttackType) {
                    case IceAttackType.FallingIcicles:
                        m_Icicles = LinealFire(new Vector2(0f, 1f), Spread, Count, Down);
                        break;
                    case IceAttackType.RadialIcicles:
                        m_Icicles = RadialFire(new Vector2(0f, 0f), Radius, Count, StartAngle, Arc);
                        break;
                    case IceAttackType.DiagonalIciclesLeft:
                        m_Icicles = LinealFire(new Vector2(0f, 1f), Spread, Count, DiagonalLeft);
                        break;
                    case IceAttackType.DiagonalIciclesRight:
                        m_Icicles = LinealFire(new Vector2(0f, 1f), Spread, Count, DiagonalRight);
                        break;
                    case IceAttackType.Shards:
                        m_Shards = ShardFire(Count);
                        break;
                }
            }
            base.Charge(deltaTime);
        }

        protected override void Release() {
            float delay = 0f;
            if (m_FlipRelease) {
                delay = m_ReleaseDelay * m_Icicles.Count;
            }
            foreach (KeyValuePair<IceQueenIcicle, Vector2> kv in m_Icicles) {
                StartCoroutine(IERelease(kv.Key, kv.Value, delay));
                delay += m_FlipRelease ? -m_ReleaseDelay : m_ReleaseDelay;
            }
            m_Icicles = new Dictionary<IceQueenIcicle, Vector2>();

            delay = 0f;
            foreach (IceQueenShard shard in m_Shards) {
                StartCoroutine(IERelease(shard, delay));
            }
            m_Shards = new List<IceQueenShard>();

            base.Release();
        }

        private IEnumerator IERelease(IceQueenIcicle icicle, Vector2 direction, float delay) {
            yield return new WaitForSeconds(delay);
            icicle.Activate(direction, m_Targets);
            yield return null;
        }

        private IEnumerator IERelease(IceQueenShard shard, float delay) {
            yield return new WaitForSeconds(delay);
            shard.Activate(shard.transform.localPosition.normalized, m_Targets);
            yield return null;
        }

        protected override void WhileActive() {
            transform.eulerAngles += 360f * m_Power / m_MaxPower * Time.fixedDeltaTime * Vector3.forward;
        }
        
        protected override void Idle() {
            if (!Attached) { return; }
            Timer.CycleTicks(ref m_CycleTicks, m_Period, Time.fixedDeltaTime);
            Obstacle.Cycle(transform, m_CycleTicks, Period, Origin, m_Ellipse);
        }

        public Dictionary<IceQueenIcicle, Vector2> LinealFire(Vector2 origin, float spread, int count, float angle) {
            Dictionary<IceQueenIcicle, Vector2> dict = new Dictionary<IceQueenIcicle, Vector2>();
            for (int i = 0; i < count; i++) {
                int total = count <= 1 ? 1 : count - 1;
                float x = spread * (float)i / (float)total - spread / 2f;
                Vector3 position = origin + new Vector2(x, 0f);
                IceQueenIcicle icicle = CreateIcicle(angle, position);

                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
                dict.Add(icicle, direction);
            }
            return dict;
        }

        public Dictionary<IceQueenIcicle, Vector2> RadialFire(Vector2 origin, float radius, int count, float start, float arc) {
            Dictionary<IceQueenIcicle, Vector2> dict = new Dictionary<IceQueenIcicle, Vector2>();
            for (int i = 0; i < count; i++) {
                int total = count <= 1 ? 1 : count - 1;
                float angle = start + arc * (float)i / (float)total;
                Vector3 direction = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
                Vector3 position = (Vector3)origin + radius * direction.normalized;
                IceQueenIcicle icicle = CreateIcicle(angle, position);

                dict.Add(icicle, direction.normalized);
            }
            return dict;
        }

        private List<IceQueenShard> ShardFire(int shards) {
            int half = (int)(Mathf.Floor((float)shards / 2f));
            int otherhalf = shards - half;
            List<IceQueenShard> shardList = new List<IceQueenShard>();
            CreateShardRing(ref shardList, m_InnerRadius, m_InnerPeriod, half);
            CreateShardRing(ref shardList, m_OuterRadius, m_OuterPeriod, otherhalf);
            return shardList;
        }

        private void CreateShardRing(ref List<IceQueenShard> shardList, float radius, float period, int count) {
            for (int i = 0; i < count; i++) {
                GameObject shardObject = Instantiate(m_Shard.gameObject, transform.position, Quaternion.identity, transform);
                float angle = 360f * (float)i / (float)count;
                Vector3 position = radius * (Quaternion.Euler(0f, 0f, angle) * Vector3.up);
                shardObject.transform.localPosition = position;
                float flip = m_FlipRelease ? -1f : 1f;
                shardObject.GetComponent<IceQueenShard>().SetOrbit(flip * radius, period * (float)i / (float)count, period);
                shardObject.SetActive(true);
                shardList.Add(shardObject.GetComponent<IceQueenShard>());
            }
        }


        private IceQueenIcicle CreateIcicle(float angle, Vector3 position) {
            GameObject icicleObject = Instantiate(m_Icicle.gameObject, transform.position, Quaternion.identity, transform);
            IceQueenIcicle icicle = icicleObject.GetComponent<IceQueenIcicle>();

            icicle.transform.localPosition = position;
            icicle.transform.eulerAngles = angle * Vector3.forward;
            icicleObject.SetActive(true);
            return icicle;
        }

    }
}
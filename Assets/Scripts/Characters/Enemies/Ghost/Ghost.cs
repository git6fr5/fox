/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// A basic type of enemy that walks in
    /// between two points while waiting a little bit
    /// at the end of each point of its path.
    ///<summary>
    public class Ghost : Enemy {

        [SerializeField] private GhostSpike m_Spike;
        
        [SerializeField, ReadOnly] private bool m_Shake = false;
        [SerializeField] private Vector2 m_CachedDirection;

        [SerializeField, ReadOnly] private float m_CacheSpeed;

        [SerializeField] private bool m_SpikeEnabled = true;
        [SerializeField] private bool m_PrevImmune = true;

        // Runs once on instantiation.
        void Awake() {
            m_Origin = transform.position;
            m_CacheSpeed = GetComponent<Character>().CharacterState.Speed;
        }
        
        // Runs once every frame to update the input.
        public override void OnUpdate() {

            bool damaged = GetComponent<Character>().CharacterState.Immune;
            if (damaged) {
                if (!m_PrevImmune) {
                    transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    GetComponent<Character>().CharacterState.OnHeal();
                    GetComponent<Character>().CharacterState.SetImmuneTicks(3f);
                }

                m_Direction = (m_Origin - transform.position).normalized;
                GetComponent<Character>().CharacterState.SetSpeed(m_CacheSpeed * (m_Origin - transform.position).magnitude);

                m_PrevImmune = true;
                return;
            }
            m_PrevImmune = false;

            if (!m_Spike.Hitbox.enabled) {
                if (m_SpikeEnabled) {
                    transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                }

                m_Direction = (m_Origin - transform.position).normalized;
                GetComponent<Character>().CharacterState.SetSpeed(m_CacheSpeed * (m_Origin - transform.position).magnitude);
                m_Shake = false;
                m_SpikeEnabled = false;
                return;
            }

            m_SpikeEnabled = true;

            m_Shake = Game.Ticks % 1f < 0.33f;
            bool move = Game.Ticks % 1f < 0.66f && Game.Ticks % 1f >= 0.33f;

            bool newDirection = Game.Ticks % 1f > 0.95f;
            if (newDirection) {
                Vector3 target = m_Origin + 1.5f * (Vector3)Random.insideUnitCircle.normalized;

                Player mainPlayer = Game.MainPlayer;
                float dist = (mainPlayer.transform.position - transform.position).magnitude;
                if (dist < 8f) {
                    target = mainPlayer.transform.position + (Vector3)Random.insideUnitCircle;
                }

                m_CachedDirection = target - transform.position;
            }

            if (move) {
                m_Direction = m_CachedDirection;
                GetComponent<Character>().CharacterState.SetSpeed(m_CacheSpeed);
            }
            else if (m_Shake) {
                m_Direction = Vector2.zero;
                GetComponent<Character>().CharacterState.SetSpeed(0f);
            }
            else {
                m_Direction = m_CachedDirection;
                GetComponent<Character>().CharacterState.SetSpeed(m_CacheSpeed * 0.2f);
            }

            GetComponent<CircleCollider2D>().isTrigger = true;

        }

        // Runs once very fixed interval.
        protected override void FixedUpdate() {
            base.FixedUpdate();

            if (m_PrevImmune) {
                return;
            }

            if (!m_Spike.Hitbox.enabled && transform.localScale.x < 1f) {
                transform.localScale *= 1.01f;
                return;
            }

            if (m_Shake) {
                transform.localScale = new Vector3(1f, 1f, 1f) * Random.Range(0.85f, 1.15f);
            }
            else {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public abstract class Weapon : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // Components.
        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        
        // Parent.
        [SerializeField, ReadOnly] protected Character m_Character;
        [SerializeField, ReadOnly] protected Vector3 m_LocalOrigin;
        public bool Attached => transform.parent == m_Character?.transform;
        public Vector3 Origin => m_Character.transform.position + m_LocalOrigin;

        // Cooldown.
        [SerializeField] protected float m_Cooldown = 0.5f;
        [SerializeField, ReadOnly] protected float m_Ticks = 0f;
        public bool Active => m_Ticks != 0f;

        // Power.
        [SerializeField, ReadOnly] protected bool m_Charge;
        [SerializeField] protected float m_MaxPower;
        [SerializeField, ReadOnly] protected float m_Power;
        public bool Charging => m_Charge;
        public bool Releasing => !m_Charge && m_Power > 0f;
        public float PowerRatio => m_Power > 0f ? m_Power / m_MaxPower : 0.05f;

        // Settings.
        [SerializeField, ReadOnly] protected Vector2 m_Direction;
        [SerializeField, ReadOnly] protected List<string> m_Targets;

        // Detail.
        [SerializeField] protected AudioClip m_FireSound;
        [SerializeField] private Color m_OutlineColor;
        
        #endregion

        // Starts the weapon.
        public void OnStart(Character character, List<string> targets) {
            m_Character = character;
            m_Targets = targets;
            m_LocalOrigin = transform.localPosition;
            // Outline.Add(m_SpriteRenderer, 1f, 16f);
            // Outline.Set(m_SpriteRenderer, m_OutlineColor);
        }

        // Updates the weapon.
        public virtual void OnUpdate(float deltaTime) {
            Timer.TickDown(ref m_Ticks, deltaTime);
            if (Charging) {
                Charge(deltaTime);
            }
            else if (Releasing) {
                Release();
            }

            if (Active) {
                WhileActive();
            }
            else {
                Idle();
            }
        }

        public void Attack(bool charge, Vector2 direction, List<string> targets) {
            m_Charge = charge;
            m_Direction = direction;
            m_Targets = targets;
        }

        public void Block(bool block, Vector2 direction) {
            // m_Block = block;
            // m_BlockDirection = direction;
        }

        protected virtual void Charge(float deltaTime) {
            Timer.TickUp(ref m_Power, m_MaxPower, Time.fixedDeltaTime);
        }

        protected virtual void Release() {
            Timer.Reset(ref m_Power);
        }

        protected virtual void WhileActive() {
            // Do nothing.
        }
        
        protected virtual void Idle() {
            // Do nothing.
        }

    }

}

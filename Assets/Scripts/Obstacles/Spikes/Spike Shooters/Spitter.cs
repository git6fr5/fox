/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Character;
using Platformer.Obstacles;
using Platformer.Utilites;
using Platformer.Physics;

namespace Platformer.Obstacles {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Spitter : MonoBehaviour {

        /* --- Variables --- */
        #region Variables
        
        [SerializeField] private float m_Interval = 1f;
        
        protected SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        [SerializeField] protected float m_SpitballSpeed;
        [SerializeField, ReadOnly] protected Vector3 m_Origin;

        [SerializeField] private VisualEffect m_SpitEffect;
        [SerializeField] private AudioClip m_SpitSound;

        [SerializeField] private float m_Ticks;
        [SerializeField] protected Spitball m_Spitball;
        
        #endregion

        public void Init(int offset) {
            // 0 => peak down going up.
            // 1 => mid going up
            // 2 => peak up going down.
            // 3 => mid going down
            m_Ticks = (float)offset / 4f * m_Interval + 0.04f;
        }

        void Start() {
            m_Origin = transform.position;
        }

        void FixedUpdate() {
            bool finished = Timer.TickDown(ref m_Ticks, Time.fixedDeltaTime);
            if (finished) {
                Spit();
                m_Ticks = m_Interval;
            }
        }

        protected virtual void Spit() {
            GameObject spitObject = Instantiate(m_Spitball.gameObject, transform.position, Quaternion.identity, null);
            spitObject.SetActive(true);
            
            Spitball spitball = spitObject.GetComponent<Spitball>();
            spitball.Body.SetVelocity(Vector3.down * m_SpitballSpeed);
            spitball.Body.gravityScale = 0.5f;
        }

    }

}
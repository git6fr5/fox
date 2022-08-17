/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Input))]
    public class Character : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // Player.
        public bool IsPlayer => GetComponent<Player>() != null;
        
        // Components.
        [HideInInspector] private Rigidbody2D m_Body;
        public Rigidbody2D Body => m_Body;
        [HideInInspector] private CircleCollider2D m_CollisionFrame;
        public CircleCollider2D CollisionFrame => m_CollisionFrame;

        // Character Components.
        [HideInInspector] private Input m_Input;
        [SerializeField] private Flipbook m_Flipbook;
        public Flipbook CharacterFlipbook => m_Flipbook;
        public Input CharacterInput => m_Input;
        [SerializeField] private Controller m_Controller;
        public Controller CharacterController => m_Controller;
        [SerializeField] private State m_State;
        public State CharacterState => m_State;
        [SerializeField] private CharacterDebugger m_CharacterDebugger;
        
        #endregion
        
        void Start() {
            // Initializes the script.
            m_Body = GetComponent<Rigidbody2D>();
            m_CollisionFrame = GetComponent<CircleCollider2D>();
            m_Input = GetComponent<Input>();
            gameObject.layer = LayerMask.NameToLayer("Characters");

            // Start these dependent scripts.
            m_State.OnStart();
            m_Controller.OnStart(this, m_Body);
            m_Flipbook.OnStart();
        }

        void Update() {
            m_Input.OnUpdate();
            m_State.OnUpdate();
            m_Controller.OnUpdate(m_Body, m_Input, m_State);
            m_Flipbook.OnUpdate(Time.deltaTime);
        }

        void FixedUpdate() {
            m_CharacterDebugger.OnUpdate(transform, m_Input, m_Controller, m_State, Time.fixedDeltaTime);
            m_Controller.OnFixedUpdate(m_Body, m_CollisionFrame, m_Input, m_State, Time.fixedDeltaTime);
            m_State.OnFixedUpdate(Time.fixedDeltaTime);
        }

        public bool Damage(int damage, Vector2 direction, float force) {
            if (!m_State.Immune && !m_State.Invulnerable) {
                m_State.OnHurt(damage);
                m_Controller.Knockback(m_Body, force * direction.normalized, 0.2f);
                Die();
                return true;
            }
            return false;
        }

        public void Die() {
            if (m_State.Health <= 0) {
                StartCoroutine(IEDie());
            }
        }

        private IEnumerator IEDie() {
            // Ramp stop if this character is a player.
            if (IsPlayer) {
                Game.RampStop();
            }
            yield return new WaitForSeconds(m_State.ImmuneBuffer);

            m_State.OnDeath(transform);
            m_Controller.OnStop(m_Body);
            yield return new WaitForSeconds(m_State.DeathBuffer);

            if (m_State.CanRespawn) {
                Start();
            }
            else {
                Game.HitStop(m_State.HitStopFrames * 2);
                Destroy(gameObject);
            }
            
        }

        void OnDrawGizmos() {
            m_CharacterDebugger.Draw(transform.position + (Vector3)GetComponent<CircleCollider2D>().offset, GetComponent<CircleCollider2D>().radius);
        }

    }

}
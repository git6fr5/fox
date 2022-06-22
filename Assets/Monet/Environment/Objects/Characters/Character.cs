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
        
        // Components.
        [HideInInspector] private Rigidbody2D m_Body;
        public Rigidbody2D Body => m_Body;
        [HideInInspector] private CircleCollider2D m_CollisionFrame;
        public CircleCollider2D CollisionFrame => m_CollisionFrame;
        [HideInInspector] private Input m_Input;
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
            m_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_CollisionFrame = GetComponent<CircleCollider2D>();
            m_Input = GetComponent<Input>();
            m_State.Init();
            m_Controller.Init();
            gameObject.layer = LayerMask.NameToLayer("Characters");
        }

        void Update() {
            m_Input.OnUpdate();
            m_State.OnUpdate();
            m_Controller.OnUpdate(m_Body, m_Input, m_State);
        }

        void FixedUpdate() {
            m_CharacterDebugger.OnUpdate(transform, m_Input, m_Controller, m_State, Time.fixedDeltaTime);
            m_Controller.OnFixedUpdate(m_Body, m_CollisionFrame, m_Input, m_State, Time.fixedDeltaTime);
        }

        public void Damage(int damage, Vector2 direction, float force) {
            m_State.Hurt(damage);
            m_Controller.Knockback(m_Body, force * direction.normalized, 0.1f);
        }

        void OnDrawGizmos() {
            m_CharacterDebugger.Draw(transform.position + (Vector3)GetComponent<CircleCollider2D>().offset, GetComponent<CircleCollider2D>().radius);
        }

    }

}
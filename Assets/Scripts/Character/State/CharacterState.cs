/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;

namespace Platformer.Character {

    ///<summary>
    /// A set of data that defines the state
    /// of a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterState : MonoBehaviour {

        #region Variables

        public bool IsPlayer => this == Game.MainPlayer;

        // Components.
        private InputSystem m_Input => GetComponent<InputSystem>();
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public Rigidbody2D Body => m_Body;
        private CircleCollider2D m_Collider => GetComponent<CircleCollider2D>();
        public CircleCollider2D Collider => m_Collider;

        [SerializeField, ReadOnly] private bool m_OnGround;
        public bool OnGround => m_OnGround;

        [SerializeField, ReadOnly] private float m_DisableTicks = 0f;
        public bool Disabled => m_DisableTicks > 0f;

        // Actions.
        [SerializeField] private MoveAction m_Movement;
        [SerializeField] private JumpAction m_Jump;
        [SerializeField] private DashAction m_Dash;

        #endregion

        public void Disable(float duration) {
            Timer.Start(ref m_DisableTicks, duration);
        }

        void Update() {
            m_OnGround = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Game.Physics.CollisionLayers.Ground);
            m_Jump.Process(m_Body, m_Input, this);

        }

        void FixedUpdate() {
            Timer.TickDown(ref m_DisableTicks, Time.fixedDeltaTime);
            m_Jump.Refresh(m_Body, this, Time.fixedDeltaTime);
            m_Movement.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
        }

    }

}


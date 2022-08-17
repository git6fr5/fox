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

        // Components.
        private InputSystem m_Input => GetComponent<InputSystem>();
        public InputSystem Input => m_Input;
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public Rigidbody2D Body => m_Body;
        private CircleCollider2D m_Collider => GetComponent<CircleCollider2D>();
        public CircleCollider2D Collider => m_Collider;

        // Settings.
        public bool IsPlayer => this == Game.MainPlayer;
        [SerializeField, ReadOnly] private bool m_OnGround;
        public bool OnGround => m_OnGround;
        [SerializeField, ReadOnly] private float m_DisableTicks = 0f;
        public bool Disabled => m_DisableTicks > 0f;
        [SerializeField, ReadOnly] private bool m_MovementOverride = false;
        public bool MovementOverride => m_MovementOverride;
        [SerializeField, ReadOnly] private bool m_FallOverride = false;
        public bool FallOverride => m_FallOverride;
        
        // Actions.
        [SerializeField] private MoveAction m_Movement;
        public MoveAction Move => m_Movement;
        [SerializeField] private JumpAction m_Jump;
        public JumpAction Jump => m_Jump;
        [SerializeField] private FallAction m_Fall;
        public FallAction Fall => m_Fall;
        [SerializeField] private DashAction m_Dash;
        public DashAction Dash => m_Dash;
        [SerializeField] private HopAction m_Hop;
        public HopAction Hop => m_Hop;

        #endregion

        public void Disable(float duration) {
            Timer.Start(ref m_DisableTicks, duration);
        }

        public void OverrideMovement(bool enable) {
            m_MovementOverride = enable;
        }

        public void OverrideFall(bool enable) {
            m_FallOverride = enable;
        }

        void Update() {
            m_Jump.Process(m_Body, m_Input, this);
            m_Dash.Process(m_Body, m_Input, this);
            m_Hop.Process(m_Body, m_Input, this);
        }

        void FixedUpdate() {
            Timer.TickDown(ref m_DisableTicks, Time.fixedDeltaTime);
            m_OnGround = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Game.Physics.CollisionLayers.Ground);

            m_Jump.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Dash.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Hop.Refresh(m_Body, m_Input, this, Time.fixedDeltaTime);

            m_Movement.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
            m_Fall.Process(m_Body, m_Input, this, Time.fixedDeltaTime);
        }

    }

}


/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;
using Platformer.Obstacles;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

using Platformer.Decor;

namespace Platformer.Character {

    ///<summary>
    /// A set of data that defines the state
    /// of a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterState : MonoBehaviour {

        #region Fields

        /* --- Member Components --- */

        // The input system attached to this 
        private InputSystem m_Input => GetComponent<InputSystem>();
        public InputSystem Input => m_Input;

        // The rigidbody attached to this.
        private Rigidbody2D m_Body => GetComponent<Rigidbody2D>();
        public Rigidbody2D Body => m_Body;

        // The circle collider attached to this.
        private CircleCollider2D m_Collider => GetComponent<CircleCollider2D>();
        public CircleCollider2D Collider => m_Collider;

        // The player.
        public bool IsPlayer => this == Game.MainPlayer;

        /* --- Member Variables --- */

        // Checks whether the character is on the ground.
        [SerializeField, ReadOnly] 
        private bool m_OnGround;
        public bool OnGround => m_OnGround;

        // Checks whether the character is facing a wall.
        [SerializeField, ReadOnly] 
        private bool m_FacingWall;
        public bool FacingWall => m_FacingWall;
        public Vector3 FacingDirection => m_Input.Direction.Facing * Vector3.right;
        
        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly] 
        private float m_DisableTicks = 0f;
        public bool Disabled => m_DisableTicks > 0f;

        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly] 
        private Vector3 m_ResetPosition = new Vector3(0f, 0f, 0f);
        public Vector3 RespawnPosition => m_ResetPosition;
        
        // Actions.
        [SerializeField] 
        private Action[] m_Actions;
        public Action[] Actions => m_Actions;

        #endregion

        public void SetResetPosition(Vector3 position) {
            m_ResetPosition = position;
        }

        public void Disable(float duration) {
            Timer.Start(ref m_DisableTicks, duration);
        }

        public void Reset() {
            transform.position = m_ResetPosition;
        }

        void Update() {
            if (Disabled) { return; }
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i].InputUpdate(m_Body, m_Input, this);
            }
        }

        void FixedUpdate() {
            Timer.TickDown(ref m_DisableTicks, Time.fixedDeltaTime);
            m_OnGround = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.down, Game.Physics.CollisionLayers.Ground);
            m_FacingWall = CollisionCheck.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, FacingDirection,  Game.Physics.CollisionLayers.Ground);
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i].PhysicsUpdate(m_Body, m_Input, this, Time.fixedDeltaTime);
            }
        }

    }

}


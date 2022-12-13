/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Input;
using Platformer.Physics;
using Platformer.Utilities;
using Platformer.Character;
using Platformer.Character.Actions;

// Definitions
using Game = Platformer.GameManager;

namespace Platformer.Character {

    ///<summary>
    /// A set of data that defines the state
    /// of a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterController : MonoBehaviour {

        #region Fields

        /* --- Memmber Components --- */
        
        // The input system attached to this body.
        public InputSystem CharacterInput => GetComponent<InputSystem>();

        // The rigidbody attached to this controller.
        public Rigidbody2D CharacterBody => GetComponent<Rigidbody2D>();

        // The main collider attached to this body.
        public CircleCollider2D CharacterCollider => GetComponent<CircleCollider2D>();

        /* --- Member Data --- */

        // The default action.
        [SerializeField] 
        private DefaultAction m_DefaultAction = null;

        /* --- Member Variables --- */

        // Checks what direction the controller is facing.
        [SerializeField, ReadOnly]
        private float m_FacingDirection = 1f;
        public float FacingDirection => m_FacingDirection;

        // Whether the direction that this is facing is locked.
        [SerializeField, ReadOnly]
        private bool m_DirectionLocked = false;

        // Checks whether the character is on the ground.
        [SerializeField, ReadOnly] 
        private bool m_OnGround = false;
        public bool OnGround => m_OnGround;

        // Checks whether the character is facing a wall.
        [SerializeField, ReadOnly] 
        private bool m_FacingWall = false;
        public bool FacingWall => m_FacingWall;

        // Checks whether the character is rising.
        [SerializeField, ReadOnly] 
        private bool m_Rising = false;
        public bool Rising => m_Rising;
        
        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly] 
        private Timer m_DisableTimer = new Timer(0f, 0f);
        public bool Disabled => m_DisableTimer.Value > 0f;

        #endregion

        // Disables the controller for a duration.
        public void Disable(float duration) {
            m_DisableTimer.Start(duration);
        }

        // Lock the direction that this controller is facing.
        public void LockDirection(bool directionLocked = true, float assignDirection = 0f) {
            m_FacingDirection = assignDirection == 0f ? m_FacingDirection : assignDirection;
            m_DirectionLocked = directionLocked;
        }

        // Runs once every frame.
        void Update() {
            // Actions.
            m_DefaultAction.InputUpdate(this);
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_DisableTimer.TickDown(Time.fixedDeltaTime);

            // Checks.
            m_Rising = CharacterBody.velocity.y > 0f;
            m_FacingDirection = m_DirectionLocked ? m_FacingDirection : CharacterInput.Direction.Horizontal;
            m_OnGround = CollisionCheck.Touching(CharacterBody.position + CharacterCollider.offset, CharacterCollider.radius, Vector3.down, Game.Physics.CollisionLayers.Ground);
            m_FacingWall = CollisionCheck.Touching(CharacterBody.position + CharacterCollider.offset, CharacterCollider.radius, Vector3.right * m_FacingDirection,  Game.Physics.CollisionLayers.Ground);
            
            // Actions.
            m_DefaultAction.PhysicsUpdate(this, Time.fixedDeltaTime);
        }

    }

}


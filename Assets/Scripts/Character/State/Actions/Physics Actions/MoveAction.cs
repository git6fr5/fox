/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;
using Platformer.Physics;
using Platformer.Character;
using Platformer.Character.Input;
using Platformer.Character.Actions;

namespace Platformer.Character.Actions {

    ///<summary>
    /// Figures out how the character should move.
    ///<summary>
    [System.Serializable]
    public class MoveAction : PhysicsAction {

        [SerializeField] private float m_Speed;
        [SerializeField] private float m_Acceleration;

        // Process the physics of this action.
        public override void Process(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (state.Disabled) { return; }
            if (state.MovementOverride) { return; }

            // Cache the target and current velocities.
            float targetSpeed = input.Direction.Move * m_Speed;
            float currentSpeed = body.velocity.x;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                body.velocity = new Vector2(targetSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(currentSpeed + deltaSpeed, body.velocity.y);
            }
            
        }

    }

}
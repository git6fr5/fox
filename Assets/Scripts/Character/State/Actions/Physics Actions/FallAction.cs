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
    /// Figures out how the character should fall.
    ///<summary>
    [System.Serializable]
    public class FallAction : PhysicsAction {

        [SerializeField] private float m_AntiGravityFactor;
        [SerializeField] private float m_AntiGravityBuffer;
        [SerializeField, ReadOnly] private float m_AntiGravityTicks;

        // Process the physics of this action.
        public override void Process(Rigidbody2D body, InputSystem input, CharacterState state, float dt) {
            if (state.Disabled) { return; }
            if (state.FallOverride) { return; }

            // Set the weight to the default.
            body.gravityScale = Game.Physics.GravityScale;

            // If the body is not on the ground.
            if (!state.OnGround) { 
                
                // And it is rising.
                if (body.Rising()) {

                    // Multiply it by its weight.
                    body.gravityScale *= state.Jump.Weight;
                    Timer.Start(ref m_AntiGravityTicks, m_AntiGravityBuffer);

                    if (!input.Action0.Held) {
                        body.SetVelocity(new Vector2(body.velocity.x, body.velocity.y * 0.9f));
                    }
                    
                }
                else {
                    // If it is falling, also multiply the sink weight.
                    body.gravityScale *= (state.Jump.Weight * state.Jump.Sink);

                    // If it is still at its apex, factor this in.
                    if (!body.Rising() && m_AntiGravityTicks > 0f) {
                        body.gravityScale *= m_AntiGravityFactor;
                        Timer.TickDown(ref m_AntiGravityTicks, dt);
                    }

                }

            }
        }

    }
}
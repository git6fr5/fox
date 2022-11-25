/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// A basic type of enemy that walks in
    /// between two points while waiting a little bit
    /// at the end of each point of its path.
    ///<summary>
    public class Goblin : Enemy {

        // Waiting.
        [SerializeField] private float m_PopUpBuffer = 1.5f;
        [SerializeField] private float m_PopUpDuration = 0.75f;
        [SerializeField, ReadOnly] private float m_PopUpTicks = 0f;

        public bool Popping => m_PopUpTicks > m_PopUpBuffer - m_PopUpDuration;

        // Runs once on instantiation.
        void Awake() {
            Timer.Start(ref m_PopUpTicks, m_PopUpBuffer);
        }
        
        // Runs once every frame to update the input.
        public override void OnUpdate() {

            if (!Popping) { 
                WaitAction();
            }

            if (m_PopUpTicks < 0.25f) {
                Player mainPlayer = Game.MainPlayer;
                Player player = Search(transform.position, mainPlayer.transform.position, 6f);
                if (player != null) {
                    if (m_PopUpTicks == 0f) {
                        Player innerPlayer = Search(transform.position, mainPlayer.transform.position, 5f);
                        if (innerPlayer != null) {
                            Timer.Start(ref m_PopUpTicks, m_PopUpBuffer);
                            m_Jump = true;
                        }
                    }
                    m_Direction.x = Mathf.Sign(player.transform.position.x - transform.position.x);
                }
            }

            bool onGround = GetComponent<Character>().CharacterController.OnGround;
            GetComponent<Character>().CharacterState.SetInvulnerable(onGround);

        }

        // Runs once very fixed interval.
        protected override void FixedUpdate() {
            base.FixedUpdate();
            Timer.TickDown(ref m_PopUpTicks, Time.fixedDeltaTime);
        }

    }

}
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
    public class Bull : Enemy {

        // jWaiting.
        [SerializeField] private float m_PopUpBuffer = 1.5f;
        [SerializeField] private float m_PopUpDuration = 0.75f;
        [SerializeField, ReadOnly] private float m_PopUpTicks = 0f;

        public bool Popping => m_PopUpTicks > m_PopUpBuffer - m_PopUpDuration;
        [SerializeField, ReadOnly] private float m_CachedDirection = 1f;

        [HideInInspector] private bool m_OnGround = true;

        // Runs once on instantiation.
        void Awake() {
            Timer.Start(ref m_PopUpTicks, m_PopUpBuffer);
        }
        
        // Runs once every frame to update the input.
        public override void OnUpdate() {

            bool onGround = GetComponent<Character>().CharacterController.OnGround;
            float antiGravTicks = GetComponent<Character>().CharacterController.AntiGravTicks;
            Rigidbody2D body = GetComponent<Character>().Body;

            if (onGround) { 
                if (m_PopUpTicks > 0.25f) {
                    WaitAction();
                }
                else {
                    PatrolAction();
                }
            }
            else if (!onGround && body.velocity.y < 0f && antiGravTicks > 0f) {
                m_Attack = true;
                m_Direction.x = 0f;
            }
            else {
                m_Attack = true;
                m_Direction.x = m_CachedDirection;
            }

            if (m_PopUpTicks == 0f) {
                Player mainPlayer = Game.MainPlayer;
                Player player = Search(transform.position, mainPlayer.transform.position, 6f);
                if (player != null) {
                    m_CachedDirection = Mathf.Sign(player.transform.position.x - transform.position.x);
                }
                else {
                    m_CachedDirection = m_Direction.x;
                }
                Timer.Start(ref m_PopUpTicks, m_PopUpBuffer);
                m_Attack = true;
                m_Jump = true;
            }

            bool land = onGround && !m_OnGround;
            if (land) {
                Screen.Shake(0.5f, 0.15f);
                m_Attack = false;
            }
            m_OnGround = onGround;

            m_HoldJump = true;

        }

        // Runs once very fixed interval.
        protected override void FixedUpdate() {
            base.FixedUpdate();
            Timer.TickDown(ref m_PopUpTicks, Time.fixedDeltaTime);
        }

    }

}
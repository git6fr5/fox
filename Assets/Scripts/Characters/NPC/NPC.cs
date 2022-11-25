/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;
using Monet.UI;

namespace Monet {

    ///<summary>
    /// An NPC that can be interacted with and talked to.
    ///<summary>
    public class NPC : Input {

        // Components.
        public Rigidbody2D Body => GetComponent<Rigidbody2D>();

        // Patrol path.
        [HideInInspector] protected Vector3 m_Origin;
        [SerializeField] protected Vector3[] m_Path;
        [SerializeField, ReadOnly] protected int m_PathIndex;

        // Waiting.
        public bool Waiting => m_WaitTicks != 0f;
        public float m_WaitBuffer = 1f;
        [SerializeField] private float m_WaitTicks;

        // Interaction.
        [SerializeField] private Twine m_StartingTwine;
        private bool m_Hover => Game.MainPlayer.ActiveNPC == this;
        private bool m_Active => m_Hover && ChatUI.MainChatUI.PromptObject.activeSelf;

        // Runs once on the first frame.
        void Start() {
            Timer.Start(ref m_WaitTicks, m_WaitBuffer);
            m_Origin = transform.position;
            m_Path = new Vector3[2] { m_Origin + 2f * Vector3.right, m_Origin - 2f * Vector3.right };
            Body.simulated = true;
        }

        // Runs once very fixed interval.
        void FixedUpdate() {
            // Calculate these values.
            float distance = Mathf.Abs(m_Path[m_PathIndex].x - transform.position.x);
            float dx = Mathf.Abs(Body.velocity.x) * Time.fixedDeltaTime;
            bool finished = Timer.TickDownIf(ref m_WaitTicks, Time.fixedDeltaTime, Waiting);

            // Start the timer if the close enough to the target.
            Timer.StartIf(ref m_WaitTicks, m_WaitBuffer, distance < dx && !Waiting);
            // Cycle the patrol array index if the timer has finished ticking down.
            Utilities.CycleIndexIf(ref m_PathIndex, 1, m_Path.Length, finished);
        }

        // Runs once every frame.
        public override void OnUpdate() {
            if (m_Hover) {
                WaitAction();
                // FacePlayer();
            }
            else {
                PatrolAction();
            }
        }

        // Make this NPC face towards the player.
        private void FacePlayer() {
            float direction = (Game.MainPlayer.transform.position.x - transform.position.x);
            float angle = 0f;
            if (direction < 0f) {
                angle = 180f;
            } 
            transform.eulerAngles = angle * Vector3.up;
        }

        // Patrols between two points.
        protected void PatrolAction() {
            if (Waiting) {
                WaitAction();
            }
            else {
                PathAction();
            }
        }

        // Waits.
        protected void WaitAction() {
            m_Direction = Vector2.zero;
            m_Attack = false;
            m_Jump = false;
            m_HoldJump = false;
        }

        // The logic for when this NPC is interacted with.
        public void Interact() {
            // TODO: Move player to interact radius.
            m_StartingTwine.Play();
        }

        // Moves to a point.
        protected void PathAction() {
            m_Direction = (Vector2)(m_Path[m_PathIndex] - transform.position);
            m_Attack = false;
            m_Jump = false;
            m_HoldJump = false;
        }

    }
}
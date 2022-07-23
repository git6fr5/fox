/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Shooter : Enemy {

        [SerializeField, ReadOnly] private Player m_Player;
        [SerializeField] private float m_PassiveSearchDistance;
        [SerializeField] private float m_ActiveSearchDistance;

        // Action Pattern.
        [SerializeField] private ActionPattern m_ActionPattern;

        public override void OnUpdate() {

            // Check for the player.
            bool playerWasNull = m_Player == null;
            float distance = playerWasNull ? m_PassiveSearchDistance : m_ActiveSearchDistance;
            m_Player = Search(transform.position, Game.MainPlayer.transform.position, distance);
            
            // If there is no player.
            if (m_Player == null) {
                PatrolAction();
            }
            else {

                // If this is the first frame that a player has been seen.
                if (playerWasNull) {
                    m_ActionPattern.OnStart();
                }

                // Get the action.
                ActionPattern.Action action = m_ActionPattern.OnUpdate(Time.deltaTime);
                if (action == null) {
                    WaitAction();
                }
                else {
                    ReadAction(action);
                }

            }
            
        }

        private void ReadAction(ActionPattern.Action action) {
            if (action.MoveTowardsPlayer) {
                Vector2 direction = (m_Player.transform.position - transform.position);
                if (direction.magnitude > action.Range) {
                    m_Direction = (direction.x * Vector3.right).normalized;
                }
                else {
                    m_Direction = Vector2.zero;
                }
            }
            else if (!action.MaintainDirection) {
                m_Direction = action.Direction;
            }
            m_Jump = action.Jump;
            m_HoldJump = action.HoldJump;
            m_Attack = action.Attack;
            if (action.AttackTowardsPlayer) {
                m_AttackDirection = (m_Player.transform.position - transform.position).normalized;
                if (action.AttackOnlyHorizontal) {
                    m_AttackDirection = (m_AttackDirection.x * Vector3.right).normalized;
                }
            }
            else {
                m_AttackDirection = action.AttackDirection;
            }
            m_Dash = action.Dash;
            if (action.DashTowardsPlayer) {
                m_DashDirection = (m_Player.transform.position - transform.position).normalized;
                if (action.DashOnlyHorizontal) {
                    m_DashDirection = (m_DashDirection.x * Vector3.right).normalized;
                }
            }
            else {
                m_DashDirection = action.DashDirection;
            }
        }

        private Player Search(Vector3 origin, Vector3 target, float maxDistance) {
            float distance = (target - origin).magnitude;
            if (distance > maxDistance) {
                return null;
            }

            // Set up the ray.
            Vector3 direction = (target - origin).normalized;
            origin += direction * 0.5f;
            distance -= 0.5f;

            // Cast the ray.
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, Game.Physics.CollisionLayers.Opaque);
            for (int i = 0; i < hits.Length; i++) {
                RaycastHit2D hit = hits[i];
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    return null;
                }
            }
            
            return Game.MainPlayer;
        }


        // For debugging.
        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_PassiveSearchDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_ActiveSearchDistance);
        }

    }
}
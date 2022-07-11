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

        [SerializeField] private ActionPattern m_ActionPattern;

        public override void OnUpdate() {
            bool playerWasNull = m_Player == null;
            float distance = playerWasNull ? m_PassiveSearchDistance : m_ActiveSearchDistance;
            m_Player = Search(transform.position, Game.MainPlayer.transform.position, distance);
            
            if (m_Player == null) {

                Target();
                m_Direction = (Vector2)(m_Path[m_PathIndex] - transform.position);
                m_Jump = false;
                m_HoldJump = false;
                
            }
            else {
                if (playerWasNull) {
                    m_ActionPattern.OnStart();
                }

                ActionPattern.Action action = m_ActionPattern.OnUpdate(Time.deltaTime);
                if (action == null) {
                    m_Direction = Vector2.zero;
                    m_Jump = false;
                    m_HoldJump = false;
                }
                else {
                    if (action.MoveTowardsPlayer) {
                        Vector2 direction = (m_Player.transform.position - transform.position);
                        if (direction.magnitude > action.Range) {
                            m_Direction = (direction.x * Vector3.right).normalized;
                        }
                        else {
                            m_Direction = Vector2.zero;
                        }
                    }
                    else {
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

            }

            
        }

        // Sets the target for this enemy.
        protected void Target() {
            float distance = (m_Path[m_PathIndex] - transform.position).magnitude;
            if (distance < Game.Physics.MovementPrecision) {
                m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
            }
        }

        private Player Search(Vector3 origin, Vector3 target, float distance) {
            // Set up the ray.
            Vector3 direction = (target - origin).normalized;
            origin += direction * 2;

            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, Game.Physics.CollisionLayers.Characters);
            if (hit.collider != null && hit.collider.gameObject != gameObject) {
                distance = (transform.position - (Vector3)hit.point).magnitude;

                Player player = hit.collider.GetComponent<Player>();
                if (player != null) {
                    return player;
                }
                
            }

            return null;
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
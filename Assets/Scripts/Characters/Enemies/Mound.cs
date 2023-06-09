/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    /// The most basic type of enemy that walks in
    /// between two points while waiting a little bit
    /// at the end of each point of its path.
    ///<summary>
    public class Mound : Enemy {

        private static float MinDistance = 1.75f;

        [SerializeField] private GameObject m_HidingObject;

        void Awake() {
            m_Origin = transform.position;
            m_Path = new Vector3[1] { m_Origin };
            m_HidingObject.transform.SetParent(transform.parent);
        }

        // Runs once every frame to update the input.
        public override void OnUpdate() {
            Shake();
        }

        public void Shake() {
            // Cache player components.
            Vector3 playerPos =  Game.MainPlayer.transform.position;
            Rigidbody2D playerBody = Game.MainPlayer.GetComponent<Rigidbody2D>();

            // Cache player components.
            float distance = Mathf.Abs(transform.position.x - playerPos.x);
            bool active = Mathf.Abs(playerBody.velocity.x) > Game.Physics.MovementPrecision;
            
            // Shake the object.
            if (active || distance < MinDistance) {
                // Get the shake strength.
                distance = distance < MinDistance ? MinDistance : distance;
                float strength = Mathf.Sqrt(MinDistance / distance);
                Obstacle.HorizontalShake(transform, m_Origin, 0.175f * strength);
                // Make noise,
                // Play effect
            }
            else {
                Obstacle.Shake(transform, m_Origin, 0f);
            }
        }

        public void OnDestroy() {
            m_HidingObject.SetActive(true);
        }

    }
}
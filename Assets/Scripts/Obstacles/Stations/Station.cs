/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Station : MonoBehaviour {

        [SerializeField] private bool m_OnlyForPlayers;

        void Awake() {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            Player player = collider.GetComponent<Player>();
            Character character = collider.GetComponent<Character>();
            if (character != null && (player != null || !m_OnlyForPlayers)) {
                Activate(character.CharacterState);
            }
        }

        protected abstract void Activate(State state);

    }
}
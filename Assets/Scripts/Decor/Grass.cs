/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Platformer.
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    ///<summary>
    /// Decorates a level with grass.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Grass : MonoBehaviour {

        // Components.
        private SpriteRenderer m_SpriteRenderer => GetComponent<SpriteRenderer>();
        [SerializeField] private Sprite[] m_Variations;

        // Runs once on instantiation.
        void Start() {
            m_SpriteRenderer.sortingLayerName = Screen.RenderingLayers.Foreground;
            m_SpriteRenderer.color = Screen.ForegroundColorShift;
            Pick();
        }

        // Picks a random grass sprite.
        public void Pick() {
            // TODO: Implement
            int index = Random.Range(0, m_Variations.Length); // Utilities.RandomIndex(m_Variations.Length);
            m_SpriteRenderer.sprite = m_Variations[index];
        }

        // Rotate the grass to face the correct direction.
        public void Rotate() {
            // TODO: Implement
        }

    }

}
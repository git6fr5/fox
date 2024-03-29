/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Decor : MonoBehaviour {

        // Components.
        [HideInInspector] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Sprite[] m_Variations;

        void Start() {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            if (m_SpriteRenderer.sortingLayerName == "Foreground") {
                m_SpriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            }
            Pick();
        }

        public void Pick() {
            int index = Utilities.RandomIndex(m_Variations.Length);
            m_SpriteRenderer.sprite = m_Variations[index];
        }

    }

}
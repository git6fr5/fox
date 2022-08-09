/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Rendering;

namespace Platformer.Rendering {

    ///<summary>
    /// Stores a set of rendering layers for easy reference.
    ///<summary>
    [System.Serializable]
    public class RenderingLayer {

        // Rendering Layers.
        [SerializeField] private string m_Background = "Background";
        public string Background => m_Background;
        [SerializeField] private string m_Midground = "Midground";
        public string Midground => m_Midground;
        [SerializeField] private string m_Foreground = "Foreground";
        public string Foreground => m_Foreground;
        [SerializeField] private string m_UI = "UI";
        public string UI => m_UI;

    }

}
    
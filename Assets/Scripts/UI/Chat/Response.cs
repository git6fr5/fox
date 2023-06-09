/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class Response {

        // The text in this response.
        [SerializeField] private string m_Text;
        public string Text => m_Text;
        
        // The twine which selecting this response leads to.
        [SerializeField] private Twine m_NextTwine;
        public Twine NextTwine => m_NextTwine;

        public Response(string text) {
            m_Text = text;
        }

    }

}
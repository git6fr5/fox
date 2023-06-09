/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    public class ResponseButton : Button {

        // The default size of a response button.
        public static Vector2 DefaultSize = new Vector2(500f, 100f);
        
        // The twine which selecting this response leads to.
        [SerializeField] private Twine m_NextTwine;
        [HideInInspector] private Response m_Response;
        public Response StoredResponse => m_Response;

        // Sets the next twine for this 
        public void SetNextTwine(Twine twine, Response response) {
            m_NextTwine = twine;
            m_Response = response;
        }

        // On selecting this button.
        public override void Activate() {
            ChatUI.Reset();
            if (m_NextTwine == null) {
                // End conversation.
            }
            else {
                m_NextTwine.Play();
            }
        }

    }
}
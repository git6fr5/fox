/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class LinearDust : Dust {

        [SerializeField] private Vector3 direction;

        protected override void AdjustPosition(float deltaTime) {
            Vector3 deltaPosition = direction * m_Speed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                m_Sparkles[i].transform.position += deltaPosition;
            }
        }

    }

}
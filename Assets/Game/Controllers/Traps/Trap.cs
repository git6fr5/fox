/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// <summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Trap : MonoBehaviour {

    bool m_Triggered;
    
    void Update() {
        m_Triggered = Trigger();
        if (m_Triggered) {
            Activate();
        }
    }

    protected abstract bool Trigger();

    protected abstract void Activate();

}

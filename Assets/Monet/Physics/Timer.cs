using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Timer {

    /* --- Variables --- */
    #region Variables
    
    public bool Running => m_Ticks > 0f;
    public bool Complete => IsComplete();

    [SerializeField, ReadOnly] private bool m_Loop;
    [SerializeField, ReadOnly] private float m_Duration = 1f;
    [SerializeField, ReadOnly] private float m_Ticks = -1f;
    
    #endregion

    public Timer(float duration, bool loop = false) {
        m_Duration = duration;
        m_Loop = loop;
        m_Ticks = 0f;
    }

    public void Start() {
        m_Ticks = 0f;
    }

    public void Update(float deltaTime) {
        m_Ticks += deltaTime;
    }

    public void Stop() {
        m_Ticks = -1f;
    }

    public bool IsComplete() {
        if (!Running) {
            return false; 
        }

        if (m_Ticks >= m_Duration) {
            if (m_Loop) { 
                m_Ticks -= m_Duration; 
            }
            else {
                Stop();
            }
            return true;
        }
        return false;

    }

}

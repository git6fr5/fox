/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Lever : MonoBehaviour {

    public List<ElevatorPlatform> m_Elevators = new List<ElevatorPlatform>();
    public List<SwitchSpike> m_SwitchSpikes = new List<SwitchSpike>();

    /* --- Unity --- */
    #region Unity
    
    public void Init(List<ElevatorPlatform> elevators, List<SwitchSpike> switchSpikes) {
        m_Elevators = elevators;
        m_SwitchSpikes = switchSpikes;
    }
    
    public void Activate() {
        for (int i = 0; i < m_Elevators.Count; i++) {
            m_Elevators[i].SwitchState();
        }

         for (int i = 0; i < m_SwitchSpikes.Count; i++) {
            m_SwitchSpikes[i].SwitchState();
        }
    }
    
    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
    }
    
    #endregion

}
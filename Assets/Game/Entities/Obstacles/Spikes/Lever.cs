/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

///<summary>
///
///<summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Lever : MonoBehaviour {

    public List<ElevatorPlatform> m_Elevators = new List<ElevatorPlatform>();
    public List<SwitchSpike> m_SwitchSpikes = new List<SwitchSpike>();

    [SerializeField, ReadOnly] private int m_Index;
    [HideInInspector] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite[] m_Sprites;
    [SerializeField] private VisualEffect m_ActivateEffect;

    /* --- Unity --- */
    #region Unity
    
    public void Init(List<ElevatorPlatform> elevators, List<SwitchSpike> switchSpikes) {
        m_Elevators = elevators;
        m_SwitchSpikes = switchSpikes;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Activate() {
        for (int i = 0; i < m_Elevators.Count; i++) {
            m_Elevators[i].SwitchState();
        }

        for (int i = 0; i < m_SwitchSpikes.Count; i++) {
            m_SwitchSpikes[i].SwitchState();
        }

        m_Index = (m_Index + 1) % m_Sprites.Length;
        m_SpriteRenderer.sprite = m_Sprites[m_Index];
        if (m_ActivateEffect != null) {
            m_ActivateEffect.Play();
        }
    }
    
    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
    }
    
    #endregion

}
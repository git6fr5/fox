using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Particle : MonoBehaviour {

    #region Variables

    [HideInInspector] public SpriteRenderer m_SpriteRenderer;

    [SerializeField] private Sprite[] m_Sprites;
    [SerializeField] public float m_Ticks;
    [SerializeField] public bool m_Loop;
    [SerializeField] public bool m_Pause;
    [SerializeField] public bool m_PauseOnEnd;

    #endregion

    /* --- Unity --- */
    #region Unity

    private void Start() {
        Init();
    }

    private void Update() {
        float deltaTime = Time.deltaTime;
        if (!m_Pause) {
            Animate(deltaTime);
            bool isEnd = m_SpriteRenderer.sprite == m_Sprites[m_Sprites.Length - 1];
            if (m_PauseOnEnd && isEnd) {
                m_Pause = true;
            }
        }
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Animate(float deltaTime) {
        // Set the current frame.
        m_Ticks += deltaTime;
        int index = (int)Mathf.Floor(m_Ticks * GameRules.FrameRate);
        if (!m_Loop && index >= m_Sprites.Length) {
            Destroy(gameObject);
        }
        index = index % m_Sprites.Length;
        m_SpriteRenderer.sprite = m_Sprites[index];
    }

    #endregion

}

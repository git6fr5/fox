/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(SpriteRenderer))]
public class Spritesheet : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    // Components.
    [Space(2), Header("Components")]
    [HideInInspector] public SpriteRenderer m_SpriteRenderer;
    [HideInInspector] public Controller m_Controller;
    [SerializeField] private Sprite[] m_Sprites;

    /* --- Parameters --- */
    [Space(2), Header("Parameters")]
    [SerializeField] private int m_IdleFrames;
    [SerializeField] private int m_MovementFrames;
    [SerializeField] private int m_RisingFrames;
    [SerializeField] private int m_FallingFrames;
    [SerializeField] private int m_DeathFrames;

    // Animations
    [HideInInspector] private Sprite[] m_IdleAnimation;
    [HideInInspector] private Sprite[] m_MovementAnimation;
    [HideInInspector] private Sprite[] m_RisingAnimation;
    [HideInInspector] private Sprite[] m_FallingAnimation;
    [HideInInspector] private Sprite[] m_DeathAnimation;

    [Space(2), Header("Animation Data")]
    [HideInInspector] private Sprite[] m_CurrentAnimation;
    [HideInInspector] private Sprite[] m_PreviousAnimation;
    [SerializeField, ReadOnly] private int m_CurrentFrame;
    [SerializeField, ReadOnly] private float m_Ticks;
    [SerializeField, ReadOnly] private float m_FrameRate;
    
    #endregion
    
    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    void Update() {
        float deltaTime = Time.deltaTime;
        if (m_Sprites != null && m_Sprites.Length > 0) {
            Animate(deltaTime);
        }
        Rotate();
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Init() {
        // Caching components.
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Controller = GetComponent<Controller>();
        if (m_Sprites != null && m_Sprites.Length > 0) {
            Organize();
        }
    }

    // Organizes the sprite sheet into its animations.
    public void Organize() {
        int startIndex = 0;
        startIndex = SliceSheet(startIndex, m_IdleFrames, ref m_IdleAnimation);
        startIndex = SliceSheet(startIndex, m_MovementFrames, ref m_MovementAnimation);
        startIndex = SliceSheet(startIndex, m_RisingFrames, ref m_RisingAnimation);
        startIndex = SliceSheet(startIndex, m_FallingFrames, ref m_FallingAnimation);
        startIndex = SliceSheet(startIndex, m_DeathFrames, ref m_DeathAnimation);

    }

    // Slices an animation out of the the sheet.
    private int SliceSheet(int startIndex, int length, ref Sprite[] array) {
        List<Sprite> splicedSprites = new List<Sprite>();
        for (int i = startIndex; i < startIndex + length; i++) {
            splicedSprites.Add(m_Sprites[i]);
        }
        array = splicedSprites.ToArray();
        return startIndex + length;
    }

    #endregion

    /* --- Rendering --- */
    #region Rendering

    private void Animate(float deltaTime) {
        m_CurrentAnimation = GetAnimation();

        m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
        m_CurrentFrame = (int)Mathf.Floor(m_Ticks * m_FrameRate) % m_CurrentAnimation.Length;

        m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];
        m_PreviousAnimation = m_CurrentAnimation;
    }

    // Gets the current animation info.
    public Sprite[] GetAnimation() {
        m_FrameRate = GameRules.FrameRate;
        if (m_Controller.AirborneFlag != Controller.Airborne.Grounded) {
            switch (m_Controller.AirborneFlag) {
                case Controller.Airborne.Rising:
                    return m_RisingAnimation;
                default:
                    return m_FallingAnimation;
            }
        }
        else if (m_Controller.MovementFlag == Controller.Movement.Moving) {
            m_FrameRate *= 2f;
            return m_MovementAnimation;
        }
        else {
            return m_IdleAnimation;
        }
    }

    private void Rotate() {
        if (m_Controller.DirectionFlag == Controller.Direction.Left) {
            transform.eulerAngles = 180f * Vector3.up;
        }
        else {
            transform.eulerAngles = Vector3.zero;
        }
    }
    
    public void AfterImage(float delay, float transparency) {
        SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        afterImage.transform.position = transform.position;
        afterImage.transform.localRotation = transform.localRotation;
        afterImage.transform.localScale = transform.localScale;
        afterImage.sprite = m_SpriteRenderer.sprite;
        afterImage.color = Color.white * transparency;
        afterImage.sortingLayerName = m_SpriteRenderer.sortingLayerName;
        Destroy(afterImage.gameObject, delay);
    }

    #endregion

}

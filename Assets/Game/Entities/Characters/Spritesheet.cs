/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.VFX;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(SpriteRenderer))]
public class Spritesheet : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    // Components.
    [HideInInspector] public SpriteRenderer m_SpriteRenderer;
    [HideInInspector] public Controller m_Controller;
    [SerializeField] protected Sprite[] m_Sprites;
    [SerializeField] protected Material[] m_Materials;

    // Frames
    [Space(2), Header("Frames")]
    [SerializeField] private int m_IdleFrames;
    [SerializeField] private int m_MovementFrames;
    [SerializeField] private int m_RisingFrames;
    [SerializeField] private int m_FallingFrames;
    [SerializeField] private int m_DeathFrames;
    [SerializeField] private int m_DoubleJumpFrames;
    [SerializeField] private int m_WallClimbIdleFrames;
    [SerializeField] private int m_WallClimbMovingFrames;
    [SerializeField] private int m_WallClimbRisingFrames;
    [SerializeField] private int m_WallClimbFallingFrames;
    [SerializeField] private int m_DashFrames;

    // Effects.
    [Space(2), Header("Frames")]
    [SerializeField] private VisualEffect m_DoubleJumpEffect;
    [SerializeField] private VisualEffect m_DashEffect;

    // Animations
    [HideInInspector] private Sprite[] m_IdleAnimation;
    [HideInInspector] private Sprite[] m_MovementAnimation;
    [HideInInspector] private Sprite[] m_RisingAnimation;
    [HideInInspector] private Sprite[] m_FallingAnimation;
    [HideInInspector] private Sprite[] m_DeathAnimation;
    [HideInInspector] private Sprite[] m_DoubleJumpAnimation;
    [HideInInspector] private Sprite[] m_WallClimbIdleAnimation;
    [HideInInspector] private Sprite[] m_WallClimbMovingAnimation;
    [HideInInspector] private Sprite[] m_WallJumpRisingAnimation;
    [HideInInspector] private Sprite[] m_WallJumpFallingAnimation;
    [HideInInspector] private Sprite[] m_DashAnimation;

    // Animation Data
    [HideInInspector] protected Sprite[] m_CurrentAnimation;
    [HideInInspector] private Sprite[] m_PreviousAnimation;
    [SerializeField, ReadOnly] private int m_CurrentFrame;
    [SerializeField, ReadOnly] private float m_Ticks;
    [SerializeField, ReadOnly] protected float m_FrameRate;

    // Animation Conditions.
    private bool Dashing => m_Controller.State.Dashing;
    private bool DoubleJumped => !m_Controller.State.DoubleJumpReset;
    private bool MovingOnGround => m_Controller.State.CanJump && m_Controller.State.Moving;
    private bool RisingJump => !m_Controller.State.CanJump && m_Controller.State.Rising;
    private bool FallingJump => !m_Controller.State.CanJump && !m_Controller.State.Rising;
    
    // Effect Conditions.
    private bool JustDashed => Dashing && !(m_PreviousAnimation == m_DashAnimation);
    private bool JustDoubleJumped => DoubleJumped && !(m_PreviousAnimation == m_DoubleJumpAnimation);

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

    public virtual void Init() {
        // Caching components.
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Controller = GetComponent<Controller>();
        if (m_Sprites != null && m_Sprites.Length > 0) {
            Organize();
        }
        m_SpriteRenderer.materials = m_Materials;
    }

    // Organizes the sprite sheet into its animations.
    public virtual int Organize() {
        int startIndex = 0;
        startIndex = SliceBasic(startIndex);
        startIndex = SliceSpecial(startIndex);

        return startIndex;
    }

    // Slices an animation out of the the sheet.
    protected int SliceSheet(int startIndex, int length, ref Sprite[] array) {
        List<Sprite> splicedSprites = new List<Sprite>();
        for (int i = startIndex; i < startIndex + length; i++) {
            splicedSprites.Add(m_Sprites[i]);
        }
        array = splicedSprites.ToArray();
        return startIndex + length;
    }

    private int SliceBasic(int startIndex) {
        startIndex = SliceSheet(startIndex, m_IdleFrames, ref m_IdleAnimation);
        startIndex = SliceSheet(startIndex, m_MovementFrames, ref m_MovementAnimation);
        startIndex = SliceSheet(startIndex, m_RisingFrames, ref m_RisingAnimation);
        startIndex = SliceSheet(startIndex, m_FallingFrames, ref m_FallingAnimation);
        startIndex = SliceSheet(startIndex, m_DeathFrames, ref m_DeathAnimation);
        return startIndex;
    }

    private int SliceSpecial(int startIndex) {
        startIndex = SliceSheet(startIndex, m_DoubleJumpFrames, ref m_DoubleJumpAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbIdleFrames, ref m_WallClimbIdleAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbMovingFrames, ref m_WallClimbMovingAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbRisingFrames, ref m_WallJumpRisingAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbFallingFrames, ref m_WallJumpFallingAnimation);
        startIndex = SliceSheet(startIndex, m_DashFrames, ref m_DashAnimation);
        return startIndex;
    }

    #endregion

    /* --- Rendering --- */
    #region Rendering

    private void Animate(float deltaTime) {
        m_CurrentAnimation = GetAnimation();
        GetEffect();

        m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
        m_CurrentFrame = (int)Mathf.Floor(m_Ticks * m_FrameRate) % m_CurrentAnimation.Length;

        m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];
        m_PreviousAnimation = m_CurrentAnimation;
    }

    // Gets the current animation info.
    public virtual Sprite[] GetAnimation() {
        m_FrameRate = GameRules.FrameRate;

        if (Dashing) {
            return m_DashAnimation;
        }
        else if (DoubleJumped) {
            return m_DoubleJumpAnimation;
        }
        else if (RisingJump) {
            return m_RisingAnimation;
        }
        else if (FallingJump) {
            return m_FallingAnimation;
        }
        else if (MovingOnGround) {
            return m_MovementAnimation;
        }
        return m_IdleAnimation;
    }

    private void GetEffect() {
        if (JustDoubleJumped) {
            m_DoubleJumpEffect.Play();
        }
        if (JustDashed) {
            m_DashEffect.Play();
        }
    }

    protected virtual void Rotate() {

        if (m_Controller.State.Dashing) {
            // Vector2 direction = new Vector2(m_Controller.State.Direction, 0f);
            float angle = Vector2.SignedAngle(Vector2.right, m_Controller.Body.velocity);
            transform.eulerAngles = angle * (m_Controller.State.Direction * Vector3.forward);
            return;
        }

        if (m_Controller.State.Direction < 0) {
            transform.eulerAngles = 180f * Vector3.up;
        }
        else {
            transform.eulerAngles = Vector3.zero;
        }
    }
    
    public static void AfterImage(SpriteRenderer spriteRenderer, Transform transform, float delay, float transparency) {
        SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        afterImage.transform.position = transform.position;
        afterImage.transform.localRotation = transform.localRotation;
        afterImage.transform.localScale = transform.localScale;
        afterImage.sprite = spriteRenderer.sprite;
        afterImage.color = Color.white * transparency;
        afterImage.sortingLayerName = spriteRenderer.sortingLayerName;
        Destroy(afterImage.gameObject, delay);
    }

    #endregion

}

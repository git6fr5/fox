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
    [SerializeField] private int m_DoubleJumpFrames;
    [SerializeField] private int m_WallClimbIdleFrames;
    [SerializeField] private int m_WallClimbMovingFrames;
    [SerializeField] private int m_WallClimbRisingFrames;
    [SerializeField] private int m_WallClimbFallingFrames;
    [SerializeField] private int m_DashFrames;
    [SerializeField] private int m_SwimFrames;

    // Effects.
    [Space(2), Header("Frames")]
    [SerializeField] private VisualEffect m_DoubleJumpEffect;
    [SerializeField] private VisualEffect m_DashEffect;
    [SerializeField] private VisualEffect m_SplashEffect;
    [SerializeField] private VisualEffect m_StepEffect;
    [SerializeField] private VisualEffect m_ClimbingStepEffect;
    [SerializeField] private VisualEffect m_LandedEffect;
    [SerializeField] private VisualEffect m_WallJumpEffect;

    // Animations
    [HideInInspector] private Sprite[] m_IdleAnimation;
    [HideInInspector] private Sprite[] m_MovementAnimation;
    [HideInInspector] private Sprite[] m_RisingAnimation;
    [HideInInspector] private Sprite[] m_FallingAnimation;
    [HideInInspector] private Sprite[] m_DoubleJumpAnimation;
    [HideInInspector] private Sprite[] m_WallClimbIdleAnimation;
    [HideInInspector] private Sprite[] m_WallClimbMovingAnimation;
    [HideInInspector] private Sprite[] m_WallJumpRisingAnimation;
    [HideInInspector] private Sprite[] m_WallJumpFallingAnimation;
    [HideInInspector] private Sprite[] m_DashAnimation;
    [HideInInspector] private Sprite[] m_SwimAnimation;

    // Animation Data
    [HideInInspector] protected Sprite[] m_CurrentAnimation;
    [HideInInspector] private Sprite[] m_PreviousAnimation;
    [SerializeField, ReadOnly] private int m_CurrentFrame;
    [SerializeField, ReadOnly] private int m_PreviousFrame;
    [SerializeField, ReadOnly] private float m_Ticks;
    [SerializeField, ReadOnly] protected float m_FrameRate;

    // Animation Conditions.
    private bool Dashing => m_Controller.State.Dashing;
    private bool DoubleJumped => !m_Controller.State.DoubleJumpReset;
    private bool MovingOnGround => m_Controller.State.CanJump && m_Controller.State.Moving;
    private bool RisingJump => !m_Controller.State.CanJump && m_Controller.State.Rising;
    private bool FallingJump => !m_Controller.State.CanJump && !m_Controller.State.Rising;
    private bool Climbing => m_Controller.State.Climbing;
    private bool ClimbMoving => m_Controller.State.Moving;
    private bool WallJumpRising => m_Controller.State.WallJumping && m_Controller.State.Rising;
    private bool WallJumpFalling => m_Controller.State.WallJumping && !m_Controller.State.Rising;
    private bool Swimming => m_Controller.State.Swimming;
    
    // Effect Conditions.
    private bool JustLanded => m_Controller.State.CanJump && !(m_PreviousAnimation == m_MovementAnimation || m_PreviousAnimation == m_IdleAnimation);
    private bool JustWallJumped => WallJumpRising && !(m_PreviousAnimation == m_WallJumpRisingAnimation);
    private bool JustDashed => Dashing && !(m_PreviousAnimation == m_DashAnimation);
    private bool JustDoubleJumped => DoubleJumped && !(m_PreviousAnimation == m_DoubleJumpAnimation);
    private bool JustGotInWater => Swimming && !(m_PreviousAnimation == m_SwimAnimation);
    private bool Step => MovingOnGround && !(m_CurrentFrame == 0 && m_PreviousFrame != 0);
    private bool ClimbingStep => Climbing && !(m_CurrentFrame == 0 && m_PreviousFrame != 0);

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
        return startIndex;
    }

    private int SliceSpecial(int startIndex) {
        startIndex = SliceSheet(startIndex, m_DoubleJumpFrames, ref m_DoubleJumpAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbIdleFrames, ref m_WallClimbIdleAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbMovingFrames, ref m_WallClimbMovingAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbRisingFrames, ref m_WallJumpRisingAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbFallingFrames, ref m_WallJumpFallingAnimation);
        startIndex = SliceSheet(startIndex, m_DashFrames, ref m_DashAnimation);
        startIndex = SliceSheet(startIndex, m_SwimFrames, ref m_SwimAnimation);
        return startIndex;
    }

    #endregion

    /* --- Rendering --- */
    #region Rendering

    private void Animate(float deltaTime) {
        m_CurrentAnimation = GetAnimation();
        GetEffect();

        m_PreviousFrame = m_CurrentFrame;

        m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
        m_CurrentFrame = (int)Mathf.Floor(m_Ticks * m_FrameRate) % m_CurrentAnimation.Length;

        m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];
        m_PreviousAnimation = m_CurrentAnimation;
    }

    // Gets the current animation info.
    public virtual Sprite[] GetAnimation() {
        m_FrameRate = GameRules.FrameRate;
        
        if (Swimming && m_SwimAnimation != null && m_SwimAnimation.Length > 0) {
            return m_SwimAnimation;
        }
        else if (Climbing) {
            if (ClimbMoving) {
                return m_WallClimbMovingAnimation;
            }
            return m_WallClimbIdleAnimation;
        }
        else if (WallJumpRising) {
            return m_WallJumpRisingAnimation;
        }
        else if (WallJumpFalling) {
            return m_WallJumpFallingAnimation;
        }
        else if (Dashing) {
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
        if (JustLanded && m_LandedEffect != null) {
            m_LandedEffect.Play();
        }
        if (JustWallJumped && m_WallJumpEffect != null) {
            m_WallJumpEffect.Play();
        }
        if (JustDoubleJumped && m_DoubleJumpEffect != null) {
            m_DoubleJumpEffect.Play();
        }
        if (JustDashed && m_DashEffect != null) {
            m_DashEffect.Play();
        }
        if (JustGotInWater && m_SplashEffect != null) {
            m_SplashEffect.Play();
        }
        if (Step && m_StepEffect != null) {
            m_StepEffect.Play();
        }
        if (ClimbingStep && m_ClimbingStepEffect != null) {
            m_ClimbingStepEffect.Play();
        }
    }

    protected virtual void Rotate() {

        if (m_Controller.State.Dashing || m_Controller.State.Swimming) {
            // Vector2 direction = new Vector2(m_Controller.State.Direction, 0f);
            Vector2 v = m_Controller.Body.velocity;
            if (m_Controller.State.Swimming) {
                v = m_Controller.GetComponent<Input>().SwimDirection;
            }


            v.x = Mathf.Abs(v.x);
            float angle = Vector2.SignedAngle(Vector2.right, v);
            Vector3 euler = angle * Vector3.forward;
            
            if (m_Controller.State.Direction < 0f) {
                euler.y = 180f;
            }

            transform.eulerAngles = euler;

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

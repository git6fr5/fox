// TODO: Clean
/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Platformer.Decor;
using Platformer.Utilites;
using Platformer.Character;
using Platformer.Physics;
using Platformer.Rendering;

namespace Platformer.Rendering {

    ///<summary>
    /// Animates the character.
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Flipbook : MonoBehaviour {

        #region Variables

        // Components.
        [SerializeField] private CharacterState m_Character;
        [SerializeField] private Sprite[] m_Sprites;
        [HideInInspector] public SpriteRenderer m_SpriteRenderer;
        
        // Current info.
        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField, ReadOnly] private int m_CurrentFrame;
        [HideInInspector] private Sprite[] m_CurrentAnimation;
        public int CurrentFrame => m_CurrentFrame;
        public int AnimationLength => m_CurrentAnimation == null ? 1 : m_CurrentAnimation.Length;
        
        // Cached info.
        [HideInInspector] private Sprite[] m_PreviousAnimation;
        [HideInInspector] private int m_PreviousFrame;

        // After Images.
        [HideInInspector] private float m_AfterImageTicks;

        // Stretch info.
        [HideInInspector] private Vector2 m_CachedStretch = new Vector2(0f, 0f);
        public static float StretchFactor = 1f;

        // Frames
        [Space(2), Header("Frames")]
        [SerializeField] private int m_IdleFrames = 4;
        [SerializeField] private int m_MovementFrames = 4;
        [SerializeField] private int m_RisingFrames;
        [SerializeField] private int m_FallingFrames;
        [SerializeField] private int m_HurtFrames;
        [SerializeField] private int m_PredashFrames;
        [SerializeField] private int m_DashFrames;
        [SerializeField] private int m_ChargeHopFrames;
        [SerializeField] private int m_HopFrames;
        [SerializeField] private int m_ShadowDashFrames;
        [SerializeField] private int m_ShadowLockFrames;
        [SerializeField] private int m_FlyFrames;
        [SerializeField] private int m_WallClimbFrames;
        [SerializeField] private int m_WallJumpFrames;

        // Sounds.
        [Space(2), Header("Sounds")]
        [SerializeField] private AudioClip m_StepSound;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] private AudioClip m_HurtSound;
        [SerializeField] private AudioClip m_DeathSound;
        [SerializeField] private AudioClip m_DashSound;
        [SerializeField] private AudioClip m_HopSound;
        [SerializeField] private AudioClip m_ShadowDashSound;
        [SerializeField] private AudioClip m_ShadowLockSound;
        [SerializeField] private AudioClip m_ClimbStepSound;
        [SerializeField] private AudioClip m_WallJumpSound;

        // Effects.
        [Space(2), Header("Effects")]
        [SerializeField] private Dust m_StepDust;
        [SerializeField] private Dust m_JumpDust;
        [SerializeField] private Dust m_LandDust;
        [SerializeField] private SparkleController m_DashSparkle;
        [SerializeField] private SparkleController m_ChargeHopSparkle;
        [SerializeField] private SparkleController m_HopSparkle;

        // Animations
        [SerializeField, ReadOnly] private Sprite[] m_IdleAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_MovementAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_RisingAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_FallingAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_HurtAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_PredashAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_DashAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_ChargeHopAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_HopAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_ShadowDashAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_ShadowLockedAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_FlyAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_WallClimbAnimation;
        [SerializeField, ReadOnly] private Sprite[] m_WallJumpAnimation;

        // Animation Conditions.

        // Movement Conditions.
        private bool Moving => m_Character.Input.Direction.Move != 0f;
        private float Direction => m_Character.Input.Direction.Facing;
        private bool Rising => !m_Character.OnGround && m_Character.Body.Rising();
        private bool Falling => !m_Character.OnGround && !m_Character.Body.Rising();
        private bool Predashing => (m_Character.Dash.Enabled && m_Character.Dash.Predashing) || (m_Character.Shadow.Enabled && m_Character.Shadow.Predashing);
        private bool Dashing => m_Character.Dash.Enabled && m_Character.Dash.Dashing;
        private bool ChargingHop => m_Character.Hop.Enabled && m_Character.Hop.Charge != 0f;
        private bool Hopping => m_Character.Hop.Enabled && !m_Character.Hop.Refreshed && Rising;
        
        private bool ShadowDashing => m_Character.Shadow.Enabled && m_Character.Shadow.Dashing;
        private bool ShadowLocked => m_Character.Shadow.Enabled && m_Character.Shadow.Locked;

        private bool WallClimbing => m_Character.Sticky.Enabled && m_Character.Sticky.Climbing;
        private bool WallJumping => m_Character.Sticky.Enabled && m_Character.Sticky.WallJumping;
        
        private bool Flying => m_Character.Ghost.Enabled && m_Character.MovementOverride == true;

        // Landing Conditions.
        private bool CacheOnGround = true;
        private bool Jump = false;
        private bool Land = false;
        
        // Stepping Condition.
        [SerializeField] private bool m_DoubleStepSound;
        private bool StepA => m_CurrentAnimation == m_MovementAnimation && ((m_CurrentFrame == 0 && m_PreviousFrame != 0) || (m_PreviousAnimation != m_MovementAnimation));
        private bool StepB => m_CurrentAnimation == m_MovementAnimation && m_CurrentFrame == (int)Mathf.Ceil(m_MovementFrames / 2f) && m_PreviousFrame != (int)Mathf.Ceil(m_MovementFrames / 2f);
        [HideInInspector] public bool PlayGroundStepSoundA = false;
        [HideInInspector] public bool PlayGroundStepSoundB = false;
        [HideInInspector] public float GroundStepSoundVolume = 0f;

        private bool ClimbStepA => m_CurrentAnimation == m_WallClimbAnimation && ((m_CurrentFrame == 0 && m_PreviousFrame != 0) || (m_PreviousAnimation != m_WallClimbAnimation));
        private bool ClimbStepB => m_CurrentAnimation == m_WallClimbAnimation && m_CurrentFrame == (int)Mathf.Ceil(m_WallClimbFrames / 2f) && m_PreviousFrame != (int)Mathf.Ceil(m_WallClimbFrames / 2f);

        // Ability Conditions.
        private bool Dash => m_CurrentAnimation == m_PredashAnimation && m_PreviousAnimation != m_PredashAnimation;
        private bool Hop => m_CurrentAnimation == m_HopAnimation && m_PreviousAnimation != m_HopAnimation;
        private bool WallJump => m_CurrentAnimation == m_WallJumpAnimation && m_PreviousAnimation != m_WallJumpAnimation;
        
        private bool ShadowDash = false; // m_CurrentAnimation == m_PredashAnimation && m_PreviousAnimation != m_PredashAnimation;
        private bool ShadowLock = false; // m_CurrentAnimation == m_PredashAnimation && m_PreviousAnimation != m_PredashAnimation;

        #endregion

        #region Initialization

        // Runs once before the first frame.
        void Start() {
            OrganizeSprites();
        }

        private int OrganizeSprites() {
            // Guard clause to protect from slicing with no sprites.
            if (!Game.Validate<Sprite>(m_Sprites)) { return 0; }

            // Cache the sprite renderer.
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sortingLayerName = Screen.RenderingLayers.Midground;

            // Slice the sheet into the individual animations.
            int index = 0;
            index = SliceSheet(index, m_IdleFrames, ref m_IdleAnimation);
            index = SliceSheet(index, m_MovementFrames, ref m_MovementAnimation);
            index = SliceSheet(index, m_RisingFrames, ref m_RisingAnimation);
            index = SliceSheet(index, m_FallingFrames, ref m_FallingAnimation);
            index = SliceSheet(index, m_HurtFrames, ref m_HurtAnimation);
            index = SliceSheet(index, m_PredashFrames, ref m_PredashAnimation);
            index = SliceSheet(index, m_DashFrames, ref m_DashAnimation);
            index = SliceSheet(index, m_ChargeHopFrames, ref m_ChargeHopAnimation);
            index = SliceSheet(index, m_HopFrames, ref m_HopAnimation);
            index = SliceSheet(index, m_ShadowDashFrames, ref m_ShadowDashAnimation);
            index = SliceSheet(index, m_ShadowLockFrames, ref m_ShadowLockedAnimation);
            index = SliceSheet(index, m_FlyFrames, ref m_FlyAnimation);
            index = SliceSheet(index, m_WallClimbFrames, ref m_WallClimbAnimation);
            index = SliceSheet(index, m_WallJumpFrames, ref m_WallJumpAnimation);
            return index;
        }

        private int SliceSheet(int startIndex, int length, ref Sprite[] array) {
            List<Sprite> splicedSprites = new List<Sprite>();
            for (int i = startIndex; i < startIndex + length; i++) {
                splicedSprites.Add(m_Sprites[i]);
            }
            array = splicedSprites.ToArray();
            return startIndex + length;
        }

        #endregion

        #region Rendering

        void Update() {
            Animate(Time.deltaTime);
        }

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        private void Animate(float deltaTime) {
            // Guard clause to protect from animating with no sprites.
            if (!Game.Validate<Sprite>(m_Sprites)) { return; }

            // Update the current animation, frame and sprite.
            m_CurrentAnimation = GetAnimation();

            float frameRate = GetFrameRate();
            m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * frameRate) % m_CurrentAnimation.Length;
            m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];

            // Check for whtether an attack any other effects have started.
            GetJumpEffect();
            GetStepEffect();
            GetAbilityEffect();
            GetRotation();
            GetScale(deltaTime);

            // Cache the current animation and frame to check for changes.
            m_PreviousAnimation = m_CurrentAnimation;
            m_PreviousFrame = m_CurrentFrame;
        }

        // Gets the current animation info.
        public virtual Sprite[] GetAnimation() {
            if (WallJumping && Game.Validate<Sprite>(m_WallJumpAnimation)) {
                return m_WallJumpAnimation;
            }
            else if (WallClimbing && Game.Validate<Sprite>(m_WallClimbAnimation)) {
                return m_WallClimbAnimation;
            }
            else if (Predashing && Game.Validate<Sprite>(m_PredashAnimation)) {
                return m_PredashAnimation;
            }
            else if (Dashing && Game.Validate<Sprite>(m_DashAnimation)) {
                return m_DashAnimation;
            }
            else if (Rising && Game.Validate<Sprite>(m_RisingAnimation)) {
                return m_RisingAnimation;
            }
            else if (Falling && Game.Validate<Sprite>(m_FallingAnimation)) {
                return m_FallingAnimation;
            }
            else if (Moving && Game.Validate<Sprite>(m_MovementAnimation)) {
                return m_MovementAnimation;
            }
            return m_IdleAnimation;
        }

        private void GetJumpEffect() {
            Jump = Rising && CacheOnGround;
            Land = m_Character.OnGround && !CacheOnGround;
            CacheOnGround = m_Character.OnGround;

            if (Jump) {
                m_JumpDust.Activate();
                m_StepDust.Activate();
                SoundManager.PlaySound(m_JumpSound, 0.2f);
            }
            else if (Land) {
                m_LandDust.Activate();
                m_StepDust.Activate();
                SoundManager.PlaySound(m_LandSound, 0.15f);
            }
        }

        private void GetStepEffect() {
            if (StepA) {
                float vA = Random.Range(0.05f, 0.075f);
                SoundManager.PlaySound(m_StepSound, vA);
                PlayGroundStepSoundA = true;
                GroundStepSoundVolume = vA;
                m_StepDust.Activate();
            }
            else if (StepB && m_DoubleStepSound) {
                float vB = Random.Range(0.03f, 0.05f);
                SoundManager.PlaySound(m_StepSound, vB);
                PlayGroundStepSoundB = true;
                GroundStepSoundVolume = vB;
                m_StepDust.Activate();
            }

            if (ClimbStepA) {
                float vA = Random.Range(0.05f, 0.075f);
                SoundManager.PlaySound(m_ClimbStepSound, vA);
                PlayGroundStepSoundA = true;
                GroundStepSoundVolume = vA;
                m_StepDust.Activate();
            }
            else if (ClimbStepB && m_DoubleStepSound) {
                float vB = Random.Range(0.03f, 0.05f);
                SoundManager.PlaySound(m_ClimbStepSound, vB);
                PlayGroundStepSoundB = true;
                GroundStepSoundVolume = vB;
                m_StepDust.Activate();
            }

        }

        private void GetAbilityEffect() {

        }

        protected virtual void GetRotation() {
            if (m_Character.Disabled) { return; }

            float currentAngle = transform.eulerAngles.y;
            float angle = Direction < 0f ? 180f : Direction > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        protected virtual void GetScale(float deltaTime) {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Vector2 stretch = Vector2.zero;
            float factor = StretchFactor * deltaTime;
            if (Rising || Falling) {
                float x = Mathf.Abs(m_Character.Body.velocity.x) * factor;
                float y = Mathf.Abs(m_Character.Body.velocity.y) * factor;
                stretch = new Vector2((x - y) / 2f, y - x);
                transform.localScale += (Vector3)(stretch + m_CachedStretch);
            }
            m_CachedStretch = stretch;
        }

        private float GetFrameRate() {
            float frameRate = Screen.FrameRate;
            return frameRate;
        }

        #endregion

    }

}



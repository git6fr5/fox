/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Platformer.Utilites;
using Platformer.Character;
using Platformer.Physics;
using Platformer.Rendering;

namespace Platformer.Rendering {

    ///<summary>
    ///
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

        // Sounds.
        [Space(2), Header("Sounds")]
        [SerializeField] private AudioClip m_StepSound;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] private AudioClip m_HurtSound;
        [SerializeField] private AudioClip m_DeathSound;
        [SerializeField] private AudioClip m_DashSound;
        [SerializeField] private AudioClip m_HopSound;

        // Effects.
        [Space(2), Header("Effects")]
        [SerializeField] private VisualEffect m_HurtEffect;
        [SerializeField] private VisualEffect m_DeathEffect;
        [SerializeField] private VisualEffect m_StepEffect;
        [SerializeField] private VisualEffect m_JumpEffect;
        [SerializeField] private VisualEffect m_LandEffect;
        [SerializeField] private VisualEffect m_DashEffect;
        [SerializeField] private VisualEffect m_HopEffect;

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

        // Animation Conditions.

        // Movement Conditions.
        private bool Moving => m_Character.Input.Direction.Move != 0f;
        private float Direction => m_Character.Input.Direction.Facing;
        private bool Rising => !m_Character.OnGround && m_Character.Body.Rising();
        private bool Falling => !m_Character.OnGround && !m_Character.Body.Rising();
        private bool Predashing => m_Character.Dash.Enabled && m_Character.Dash.Predashing;
        private bool Dashing => m_Character.Dash.Enabled && m_Character.Dash.Dashing;
        private bool ChargingHop => m_Character.Hop.Enabled && m_Character.Hop.Charge != 0f;
        private bool Hopping => m_Character.Hop.Enabled && !m_Character.Hop.Refreshed && Rising;

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

        // Ability Conditions.
        private bool Dash => m_CurrentAnimation == m_PredashAnimation && m_PreviousAnimation != m_PredashAnimation;
        private bool Hop => m_CurrentAnimation == m_HopAnimation && m_PreviousAnimation != m_HopAnimation;

        #endregion

        /* --- Initialization --- */
        #region Initialization

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

        /* --- Rendering --- */
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
            m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * Screen.FrameRate) % m_CurrentAnimation.Length;
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
            if (ChargingHop && Game.Validate<Sprite>(m_ChargeHopAnimation)) {
                return m_ChargeHopAnimation;
            }
            else if (Hopping && Game.Validate<Sprite>(m_HopAnimation)) {
                return m_HopAnimation;
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
                if (m_JumpEffect != null) { m_JumpEffect.Play(); }
                SoundManager.PlaySound(m_JumpSound, 0.2f);
            }
            else if (Land) {
                if (m_LandEffect != null) { m_LandEffect.Play(); }
                SoundManager.PlaySound(m_LandSound, 0.15f);
            }
        }

        private void GetStepEffect() {
            if (StepA) {
                // if (m_StepEffectA != null) { m_StepEffectA.Play(); }
                float vA = Random.Range(0.05f, 0.075f);
                SoundManager.PlaySound(m_StepSound, vA);
                PlayGroundStepSoundA = true;
                GroundStepSoundVolume = vA;
            }
            else if (StepB && m_DoubleStepSound) {
                // if (m_StepEffectB != null) { m_StepEffectB.Play(); }
                float vB = Random.Range(0.03f, 0.05f);
                SoundManager.PlaySound(m_StepSound, vB);
                PlayGroundStepSoundB = true;
                GroundStepSoundVolume = vB;
            }

        }

        private void GetAbilityEffect() {
            if (Hop) {
                // if (m_StepEffectA != null) { m_StepEffectA.Play(); }
                SoundManager.PlaySound(m_HopSound, 0.15f);
            }
            else if (Dash) {
                // if (m_StepEffectB != null) { m_StepEffectB.Play(); }
                SoundManager.PlaySound(m_DashSound, 0.15f);
            }

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

        #endregion

    }

}



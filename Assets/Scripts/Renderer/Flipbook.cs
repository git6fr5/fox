/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Flipbook : MonoBehaviour {

        #region Variables

        // Components.
        [SerializeField] private Character m_Character;
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
        [HideInInspector] private bool m_PrevImmunity;
        [HideInInspector] private float m_ImmuneCycleTicks;

        // Attack info.
        [HideInInspector] private bool m_Attack;
        [HideInInspector] private bool m_PrevAttack;
        [HideInInspector] private bool m_StartedAttack;
        [HideInInspector] private bool m_FinishedAttack;

        // After Images.
        [HideInInspector] private float m_AfterImageTicks;

        // Stretch info.
        [HideInInspector] private Vector2 m_CachedStretch = new Vector2(0f, 0f);
        public static float StretchFactor = 1f;

        // Outline color.
        [SerializeField] private Color m_OutlineColor;

        // Frames
        [Space(2), Header("Frames")]
        [SerializeField] private int m_IdleFrames = 4;
        [SerializeField] private int m_MovementFrames = 4;
        [SerializeField] private int m_RisingFrames;
        [SerializeField] private int m_FallingFrames;
        [SerializeField] private int m_HurtFrames;
        [SerializeField] private int m_AttackFrames;
        [SerializeField] private int m_ChargeAttackFrames;
        [SerializeField] private int m_DoubleJumpFrames;
        [SerializeField] private int m_DashFrames;

        // Sounds.
        [Space(2), Header("Sounds")]
        [SerializeField] private AudioClip m_StepSoundA;
        [SerializeField] private AudioClip m_StepSoundB;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] private AudioClip m_DoubleJumpSound;
        [SerializeField] private AudioClip m_DashSound;
        [SerializeField] private AudioClip m_HurtSound;
        [SerializeField] private AudioClip m_DeathSound;

        // Effects.
        [Space(2), Header("Effects")]
        [SerializeField] private VisualEffect m_StepEffectA;
        [SerializeField] private VisualEffect m_StepEffectB;
        [SerializeField] private VisualEffect m_JumpEffect;
        [SerializeField] private VisualEffect m_LandEffect;
        [SerializeField] private VisualEffect m_DoubleJumpEffect;
        [SerializeField] private VisualEffect m_DashEffect;
        [SerializeField] private VisualEffect m_HurtEffect;
        [SerializeField] private VisualEffect m_DeathEffect;

        // Animations
        [HideInInspector] private Sprite[] m_IdleAnimation;
        [HideInInspector] private Sprite[] m_MovementAnimation;
        [HideInInspector] private Sprite[] m_RisingAnimation;
        [HideInInspector] private Sprite[] m_FallingAnimation;
        [HideInInspector] private Sprite[] m_HurtAnimation;
        [HideInInspector] private Sprite[] m_ChargeAttackAnimation;
        [HideInInspector] private Sprite[] m_AttackAnimation;
        [HideInInspector] private Sprite[] m_DoubleJumpAnimation;
        [HideInInspector] private Sprite[] m_DashAnimation;

        // Animation Conditions.
        private bool Knockedback => m_Character.CharacterController.Knockedback;
        private bool Immune => m_Character.CharacterState.Immune;

        // Movement Conditions.
        private bool Moving => !ChargingAttack && m_Character.CharacterInput.MoveDirection != 0f;
        private bool Attacking => m_Character.CharacterController.MainWeapon != null && !m_Character.CharacterController.MainWeapon.Charging;
        private float Direction => m_Character.CharacterInput.MoveDirection;
        private bool Rising => !m_Character.CharacterController.OnGround && m_Character.CharacterController.Rising;
        private bool Falling => !m_Character.CharacterController.OnGround && !m_Character.CharacterController.Rising;
        private bool DoubleJumping => m_Character.CharacterController.Rising && !m_Character.CharacterController.DoubleJumpReset && m_Character.CharacterController.UnlockedDoubleJump;
        private bool Dashing => m_Character.CharacterController.Knockedback && !m_Character.CharacterController.DashReset && m_Character.CharacterController.UnlockedDash;

        // Effect Conditions.
        private bool Step => m_CurrentAnimation == m_MovementAnimation && m_PreviousAnimation != m_MovementAnimation;
        private bool StepA => m_CurrentAnimation == m_MovementAnimation && m_CurrentFrame == 0 && m_PreviousFrame != 0;
        private int MidStep => (int)Mathf.Ceil(m_MovementFrames / 2f);
        private bool StepB => m_CurrentAnimation == m_MovementAnimation && m_CurrentFrame == MidStep && m_PreviousFrame != MidStep;
        private bool Jump => m_CurrentAnimation == m_RisingAnimation && m_PreviousAnimation != m_RisingAnimation;
        private bool Land => m_PreviousAnimation == m_FallingAnimation && m_CurrentAnimation != m_FallingAnimation;
        private bool DoubleJump => m_CurrentAnimation == m_DoubleJumpAnimation && m_PreviousAnimation != m_DoubleJumpAnimation;
        private bool Dash => m_CurrentAnimation == m_DashAnimation && m_PreviousAnimation != m_DashAnimation;

        // Attack Conditions.
        private bool StartedAttack => m_StartedAttack;
        private bool ChargingAttack => m_Character.CharacterController.MainWeapon != null && m_Character.CharacterInput.Attack;

        #endregion

        /* --- Initialization --- */
        #region Initialization

        public void OnStart() {
            OrganizeSprites();
            Outline.Add(m_SpriteRenderer, 0f, 16f);
            Outline.Set(m_SpriteRenderer, m_OutlineColor);
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
            index = SliceSheet(index, m_ChargeAttackFrames, ref m_ChargeAttackAnimation);
            index = SliceSheet(index, m_AttackFrames, ref m_AttackAnimation);
            index = SliceSheet(index, m_DoubleJumpFrames, ref m_DoubleJumpAnimation);
            index = SliceSheet(index, m_DashFrames, ref m_DashAnimation);
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

        public void OnUpdate(float deltaTime) {
            Animate(deltaTime);
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
            GetAttack();
            GetEffect();
            GetRotation();
            GetShake();
            GetImmune(deltaTime);
            GetScale(deltaTime);
            GetAfterImages(deltaTime);

            // Cache the current animation and frame to check for changes.
            m_PreviousAnimation = m_CurrentAnimation;
            m_PreviousFrame = m_CurrentFrame;
        }

        // Gets the current animation info.
        public virtual Sprite[] GetAnimation() {
            if (Immune && Game.Validate<Sprite>(m_HurtAnimation)) {
                return m_HurtAnimation;
            }
            else if (StartedAttack && Game.Validate<Sprite>(m_AttackAnimation)) {
                return m_AttackAnimation;
            }
            else if (ChargingAttack && Game.Validate<Sprite>(m_ChargeAttackAnimation)) {
                return m_ChargeAttackAnimation;
            }
            else if (Dashing && Game.Validate<Sprite>(m_DashAnimation)) {
                return m_DashAnimation;
            }
            else if (DoubleJumping && Game.Validate<Sprite>(m_DoubleJumpAnimation)) {
                return m_DoubleJumpAnimation;
            }
            if (Rising && Game.Validate<Sprite>(m_RisingAnimation)) {
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

        private void GetAttack() {
            m_Attack = m_Character.CharacterController.MainWeapon != null && m_Character.CharacterController.MainWeapon.Active;
            m_FinishedAttack = m_StartedAttack && m_CurrentFrame == m_AttackFrames - 1;

            m_StartedAttack = m_Attack && !m_PrevAttack ? true : (m_FinishedAttack ? false : m_StartedAttack);
            m_PrevAttack = m_Attack;
        }

        private void GetEffect() {
            if (Step || StepA) {
                if (m_StepEffectA != null) { m_StepEffectA.Play(); }
                SoundManager.PlaySound(m_StepSoundA, 0.05f);
            }
            if (StepB) {
                if (m_StepEffectB != null) { m_StepEffectB.Play(); }
                SoundManager.PlaySound(m_StepSoundB, 0.025f);
            }
            if (Jump) {
                if (m_JumpEffect != null) { m_JumpEffect.Play(); }
                SoundManager.PlaySound(m_JumpSound, 0.2f);
            }
            else if (Land) {
                if (m_LandEffect != null) { m_LandEffect.Play(); }
                SoundManager.PlaySound(m_LandSound, 0.1f);
            }
            if (DoubleJump) {
                if (m_DoubleJumpEffect != null) { m_DoubleJumpEffect.Play(); }
                SoundManager.PlaySound(m_DoubleJumpSound);
            }
            if (Dash) {
                if (m_DashEffect != null) { m_DashEffect.Play(); }
                SoundManager.PlaySound(m_DashSound);
            }

        }

        private void GetImmune(float deltaTime) {
            m_SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            if (Immune) {
                if (!m_PrevImmunity) {
                    if (m_Character.CharacterState.Health <= 0) {
                        if (m_DeathEffect != null) { m_DeathEffect.Play(); }
                        SoundManager.PlaySound(m_DeathSound);
                    }
                    else {
                        if (m_HurtEffect != null) { m_HurtEffect.Play(); }
                        SoundManager.PlaySound(m_HurtSound);
                    }
                    Outline.Set(m_SpriteRenderer, Color.white);
                }
                bool on = Timer.CycleTicks(ref m_ImmuneCycleTicks, 0.075f, deltaTime);
                m_SpriteRenderer.color = on ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
            }
            if (!Immune && m_PrevImmunity) {
                Outline.Set(m_SpriteRenderer, m_OutlineColor);
            }

            m_PrevImmunity = Immune;
        }

        protected virtual void GetRotation() {
            float currentAngle = transform.eulerAngles.y;
            float angle = Direction < 0f ? 180f : Direction > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        private void GetShake() {
            float strength = Mathf.Max(m_Character.CharacterController.JumpCharge, m_Character.CharacterController.DashCharge);
            Obstacle.Shake(transform, m_Character.transform.position, strength * 0.1f);
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

        // public GameObject eyeObj;

        private void GetAfterImages(float deltaTime) {
            if (Dashing) {
                // TODO: okay getting real lazy.
                // eyeObj.SetActive(true);
                bool finished = Timer.TickDown(ref m_AfterImageTicks, deltaTime);
                if (finished || m_AfterImageTicks <= 0f) {
                    AfterImage(m_SpriteRenderer, transform, 0.15f, 0.5f);
                    Timer.Start(ref m_AfterImageTicks, 0.03f);
                }
            }
            else {
                // eyeObj.SetActive(false);
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

}


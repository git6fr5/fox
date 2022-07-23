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
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Spritesheet : MonoBehaviour {

        // Components.
        [HideInInspector] public SpriteRenderer m_SpriteRenderer;
        [HideInInspector] public Character m_Character;

        // Animation Data
        [Space(2), Header("Data")]
        [SerializeField] private Material[] m_Materials;
        [SerializeField] private Sprite[] m_Sprites;
        [HideInInspector] private Sprite[] m_CurrentAnimation;
        [HideInInspector] private Sprite[] m_PreviousAnimation;
        [SerializeField, ReadOnly] private int m_CurrentFrame;
        [HideInInspector] private int m_PreviousFrame;
        [HideInInspector] private float m_Ticks;

        // Frames
        [Space(2), Header("Slicing")]
        [SerializeField] private int m_IdleFrames;
        [SerializeField] private int m_MovementFrames;
        [SerializeField] private int m_RisingFrames;
        [SerializeField] private int m_FallingFrames;
        [SerializeField] private int m_AttackFrames;
        [SerializeField] private int m_ChargeAttackFrames;
        [SerializeField] private int m_DoubleJumpFrames;
        [SerializeField] private int m_DashFrames;

        // Sounds.
        [SerializeField] private AudioClip m_StepSound;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] private AudioClip m_DoubleJumpSound;
        [SerializeField] private AudioClip m_DashSound;
        [SerializeField] private AudioClip m_HurtSound;
        [SerializeField] private AudioClip m_DeathSound;

        // Effects.
        [SerializeField] private VisualEffect m_StepEffect;
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
        [HideInInspector] private Sprite[] m_ChargeAttackAnimation;
        [HideInInspector] private Sprite[] m_AttackAnimation;
        [HideInInspector] private Sprite[] m_DoubleJumpAnimation;
        [HideInInspector] private Sprite[] m_DashAnimation;

        // Animation Conditions.
        private bool Knockedback => m_Character.CharacterController.Knockedback;
        private bool Immune => m_Character.CharacterState.Immune;
        private bool PrevImmunity;
        private float ImmuneCycleTicks;
        private bool Attacking => m_Character.CharacterController.MainWeapon != null && !m_Character.CharacterController.MainWeapon.CanFire;
        private bool ChargingAttack => m_Character.CharacterController.MainWeapon != null && m_Character.CharacterInput.Attack;
        private bool Moving => !ChargingAttack && m_Character.CharacterInput.MoveDirection != 0f;
        private float Direction => m_Character.CharacterInput.MoveDirection;
        private bool Rising => !m_Character.CharacterController.OnGround && m_Character.CharacterController.Rising;
        private bool Falling => !m_Character.CharacterController.OnGround && !m_Character.CharacterController.Rising;
        private bool DoubleJumping => m_Character.CharacterController.Rising && !m_Character.CharacterController.DoubleJumpReset && m_Character.CharacterController.UnlockedDoubleJump;
        private bool Dashing => m_Character.CharacterController.Knockedback && !m_Character.CharacterController.DashReset && m_Character.CharacterController.UnlockedDash;

        // Effect Conditions.
        private bool Step => m_CurrentAnimation == m_MovementAnimation && m_CurrentFrame == 0 && m_PreviousFrame != 0;
        private bool Jump => m_CurrentAnimation == m_RisingAnimation && m_PreviousAnimation != m_RisingAnimation;
        private bool Land => m_PreviousAnimation == m_FallingAnimation && m_CurrentAnimation != m_FallingAnimation;
        private bool DoubleJump => m_CurrentAnimation == m_DoubleJumpAnimation && m_PreviousAnimation != m_DoubleJumpAnimation;
        private bool Dash => m_CurrentAnimation == m_DashAnimation && m_PreviousAnimation != m_DashAnimation;

        // Attacks.
        private bool m_Attack;
        private bool m_PrevAttack;
        private bool m_StartedAttack;
        private bool m_FinishedAttack;
        
        /* --- Initialization --- */
        #region Initialization

        void Start() {
            Init();
        }

        public virtual void Init() {
            // Caching components.
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sortingLayerName = Screen.RenderingLayers.Midground;
            m_Character = GetComponent<Character>();
            if (m_Sprites != null && m_Sprites.Length > 0) {
                Organize();
            }
            m_SpriteRenderer.materials = m_Materials;
        }

        public virtual int Organize() {
            int startIndex = 0;
            startIndex = SliceSheet(startIndex, m_IdleFrames, ref m_IdleAnimation);
            startIndex = SliceSheet(startIndex, m_MovementFrames, ref m_MovementAnimation);
            startIndex = SliceSheet(startIndex, m_RisingFrames, ref m_RisingAnimation);        
            startIndex = SliceSheet(startIndex, m_FallingFrames, ref m_FallingAnimation); 
            startIndex = SliceSheet(startIndex, m_ChargeAttackFrames, ref m_ChargeAttackAnimation);     
            startIndex = SliceSheet(startIndex, m_AttackFrames, ref m_AttackAnimation);     
            startIndex = SliceSheet(startIndex, m_DoubleJumpFrames, ref m_DoubleJumpAnimation);        
            startIndex = SliceSheet(startIndex, m_DashFrames, ref m_DashAnimation);        
            return startIndex;
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
            float deltaTime = Time.deltaTime;
            if (m_Sprites != null && m_Sprites.Length > 0) {
                Animate(deltaTime);
            }
            Rotate();
        }


        private void Animate(float deltaTime) {
            m_CurrentAnimation = GetAnimation();
            m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
            m_CurrentFrame = (int)Mathf.Floor(m_Ticks * Screen.FrameRate) % m_CurrentAnimation.Length;
            m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];
            
            GetEffect();

            m_PreviousAnimation = m_CurrentAnimation;
            m_PreviousFrame = m_CurrentFrame;
        }

        // Gets the current animation info.
        public virtual Sprite[] GetAnimation() {

            m_Attack = m_Character.CharacterController.MainWeapon != null && !m_Character.CharacterController.MainWeapon.CanFire;
            m_FinishedAttack = m_StartedAttack && m_CurrentFrame == m_AttackFrames - 1;
            
            m_StartedAttack = m_Attack && !m_PrevAttack ? true : (m_FinishedAttack ? false : m_StartedAttack);
            m_PrevAttack = m_Attack;

            if (m_StartedAttack && Game.Validate<Sprite>(m_AttackAnimation)) {
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

        private void GetEffect() {
            m_SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            if (Immune) {
                if (!PrevImmunity) {
                    if (m_Character.CharacterState.Health <= 0) {
                        if (m_DeathEffect != null) { m_DeathEffect.Play(); }
                        SoundManager.PlaySound(m_DeathSound);
                    }
                    else {
                        if (m_HurtEffect != null) { m_HurtEffect.Play(); }
                        SoundManager.PlaySound(m_HurtSound);
                    }
                }
                bool on = Timer.CycleTicks(ref ImmuneCycleTicks, 0.075f, Time.deltaTime);
                m_SpriteRenderer.color = on ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
            }
            if (Step) { // && stepEFX != null
                // Effect.Play etc.
                // Where effect is both the vfx and sfx.\
                if (m_StepEffect != null) { m_StepEffect.Play(); }
                SoundManager.PlaySound(m_StepSound);
            }
            if (Jump) {
                if (m_JumpEffect != null) { m_JumpEffect.Play(); }
                SoundManager.PlaySound(m_JumpSound);
            }
            else if (Land) {
                if (m_LandEffect != null) { m_LandEffect.Play(); }
                SoundManager.PlaySound(m_LandSound);
            }
            if (DoubleJump) {
                if (m_DoubleJumpEffect != null) { m_DoubleJumpEffect.Play(); }
                SoundManager.PlaySound(m_DoubleJumpSound);
            }
            if (Dash) {
                if (m_DashEffect != null) { m_DashEffect.Play(); }
                SoundManager.PlaySound(m_DashSound);
            }

            PrevImmunity = Immune;

        }

        protected virtual void Rotate() {
            if (Direction < 0f) {
                RotateBody(m_Character.Body, 180f);
            } 
            else if (Direction > 0f) {
                RotateBody(m_Character.Body, 0f);
            }
        }

        private static void RotateBody(Rigidbody2D body, float angle) {
            if (body.transform.eulerAngles == angle * Vector3.up) { return; }
            body.constraints = RigidbodyConstraints2D.None;
            body.transform.eulerAngles = angle * Vector3.up;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
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

    
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
    public class Flipbook : MonoBehaviour {

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
        public int CurrentFrame => m_CurrentFrame;
        public int AnimationLength => m_CurrentAnimation == null ? 1 : m_CurrentAnimation.Length;
        [SerializeField, ReadOnly] private int m_PreviousFrame;
        [SerializeField, ReadOnly] private float m_Ticks;
        [SerializeField, ReadOnly] private int m_IdleIndex;

        // Frames
        [Space(2), Header("Slicing")]
        [SerializeField] private int m_IdleVariations = 1;
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
        [HideInInspector] private Sprite[] m_StaticAnimation;
        [HideInInspector] private List<Sprite[]> m_IdleAnimations;
        [HideInInspector] private Sprite[] m_MovementAnimation;
        [HideInInspector] private Sprite[] m_RisingAnimation;
        [HideInInspector] private Sprite[] m_FallingAnimation;
        [HideInInspector] private Sprite[] m_HurtAnimation;
        [HideInInspector] private Sprite[] m_ChargeAttackAnimation;
        [HideInInspector] private Sprite[] m_AttackAnimation;
        [HideInInspector] private Sprite[] m_DoubleJumpAnimation;
        [HideInInspector] private Sprite[] m_DashAnimation;

        //
        [HideInInspector] private Color m_BaseOutlineColor;

        // Animation Conditions.
        private bool Knockedback => m_Character.CharacterController.Knockedback;
        private bool Immune => m_Character.CharacterState.Immune;
        private bool PrevImmunity;
        private float ImmuneCycleTicks;

        private bool Moving => !ChargingAttack && m_Character.CharacterInput.MoveDirection != 0f;
        private bool ChargingAttack => m_Character.CharacterController.MainWeapon != null && m_Character.CharacterInput.Attack;
        private bool Attacking => m_Character.CharacterController.MainWeapon != null && !m_Character.CharacterController.MainWeapon.CanFire;
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
            // m_SpriteRenderer.materials = m_Materials;
            Outline.Add(m_SpriteRenderer, 0.5f, 16f);
            Outline.Set(m_SpriteRenderer, Color.black);
            // m_BaseOutlineColor = m_SpriteRenderer.material.GetColor("_OutlineColor"); // sprite;
        }

        public virtual int Organize() {
            int startIndex = SliceSheet(0, 1, ref m_StaticAnimation);
            m_IdleAnimations = new List<Sprite[]>();
            for (int i = 0; i < m_IdleVariations; i++) {
                Sprite[] idleAnim = new Sprite[m_IdleFrames];
                startIndex = SliceSheet(startIndex, m_IdleFrames, ref idleAnim);
                m_IdleAnimations.Add(idleAnim);
            }
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
            Stretch();
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

            if (Immune && Game.Validate<Sprite>(m_HurtAnimation)) {
                return m_HurtAnimation;
            }
            else if (m_StartedAttack && Game.Validate<Sprite>(m_AttackAnimation)) {
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
            // else if (m_IdleIndex != -1 && Game.Validate<Sprite>(m_IdleAnimations[m_IdleIndex])) {
            //     return m_IdleAnimations[m_IdleIndex];
            // }
            return m_IdleAnimations[0];
        }

        private void GetEffect() {

            m_SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            // if (m_BaseOutlineColor != null) {
            //     m_SpriteRenderer.material.SetColor("_OutlineColor", m_BaseOutlineColor);
            // }
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
                    Outline.Set(m_SpriteRenderer, Color.white);
                }
                bool on = Timer.CycleTicks(ref ImmuneCycleTicks, 0.075f, Time.deltaTime);
                m_SpriteRenderer.color = on ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
                // if (m_BaseOutlineColor != null) {
                //     m_SpriteRenderer.material.SetColor("_OutlineColor", Color.white);
                // }
            }
            if (!Immune && PrevImmunity) {
                Outline.Set(m_SpriteRenderer, Color.black);
            }

            PrevImmunity = Immune;

            if (Step) {
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
        

            if (m_IdleAnimations.Contains(m_CurrentAnimation) || m_IdleIndex == -1) {
                bool condA = m_PreviousFrame == m_CurrentAnimation.Length - 1;
                bool condB = m_CurrentFrame == 0;
                if (condA && condB) {
                    m_IdleIndex = -1;
                    float prob = Random.Range(0f, 1f);
                    if (prob > 0.995f) {
                        m_IdleIndex = Random.Range(0, m_IdleAnimations.Count);
                    }
                }
            }

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

        // [SerializeField] private Vector2 m_CachedStretch;
        // [SerializeField] private float StretchFactor = 10f;

        void Stretch() {
            // Vector2 stretch = new Vector2(1f, 1f);
            // if (Rising || Falling) {
            //     stretch.y = 0.75f;
            //     stretch.x = 1.25f;
            // }
            // m_SpriteRenderer.materials[0].SetVector("_Stretch", stretch);
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


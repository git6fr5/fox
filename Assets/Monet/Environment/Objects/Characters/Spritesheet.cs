/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        [SerializeField] private int m_DoubleJumpFrames;
        [SerializeField] private int m_DashFrames;

        // Sounds.
        [SerializeField] private AudioClip m_StepSound;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] private AudioClip m_DoubleJumpSound;
        [SerializeField] private AudioClip m_DashSound;

        // Animations
        [HideInInspector] private Sprite[] m_IdleAnimation;
        [HideInInspector] private Sprite[] m_MovementAnimation;
        [HideInInspector] private Sprite[] m_RisingAnimation;
        [HideInInspector] private Sprite[] m_FallingAnimation;
        [HideInInspector] private Sprite[] m_DoubleJumpAnimation;
        [HideInInspector] private Sprite[] m_DashAnimation;

        // Animation Conditions.
        private bool Moving => m_Character.CharacterInput.MoveDirection != 0f;
        private float Direction => m_Character.CharacterInput.MoveDirection;
        private bool Rising => !m_Character.CharacterController.OnGround && m_Character.CharacterController.Rising;
        private bool Falling => !m_Character.CharacterController.OnGround && !m_Character.CharacterController.Rising;
        private bool DoubleJumping => m_Character.CharacterController.Rising && !m_Character.CharacterController.DoubleJumpReset;
        private bool Dashing => m_Character.CharacterController.Knockedback && !m_Character.CharacterController.DashReset;

        // Effect Conditions.
        private bool Step => m_CurrentAnimation == m_MovementAnimation && m_CurrentFrame == 0 && m_PreviousFrame != 0;
        private bool Jump => m_CurrentAnimation == m_RisingAnimation && m_PreviousAnimation != m_RisingAnimation;
        private bool Land => m_PreviousAnimation == m_FallingAnimation && m_CurrentAnimation != m_FallingAnimation;
        private bool DoubleJump => m_CurrentAnimation == m_DoubleJumpAnimation && m_PreviousAnimation != m_DoubleJumpAnimation;
        private bool Dash => m_CurrentAnimation == m_DashAnimation && m_PreviousAnimation != m_DashAnimation;
        
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
            if (Dashing && Game.Validate<Sprite>(m_DashAnimation)) {
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
            if (Step) { // && stepEFX != null
                // Effect.Play etc.
                // Where effect is both the vfx and sfx.\
                SoundManager.PlaySound(m_StepSound);
            }
            if (Jump) {
                SoundManager.PlaySound(m_JumpSound);
            }
            else if (Land) {
                SoundManager.PlaySound(m_LandSound);
            }
            if (DoubleJump) {
                SoundManager.PlaySound(m_DoubleJumpSound);
            }
            if (Dash) {
                SoundManager.PlaySound(m_DashSound);
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

    
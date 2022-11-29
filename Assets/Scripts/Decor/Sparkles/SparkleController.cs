// Libraries.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Platformer.
using Platformer.Utilites;
using Platformer.Decor;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Decor {

    public class SparkleController : MonoBehaviour {

        #region Fields

        /* --- Member Components --- */

        // The sprite that this sparkle uses as a base.
        [SerializeField] 
        private Sprite m_SparkleSprite;

        // The collection of sparkles that are being rendered.
        [HideInInspector]
        protected List<SpriteRenderer> m_Sparkles = new List<SpriteRenderer>();

        /* --- Member Variables --- */

        // Checks whether the activation condition for the sparkle is met.
        private bool ActivationCondition => CheckActivationCondition();

        // The minimum sorting order that this can be rendered at.
        [SerializeField] 
        private int m_MinSortingOrder;

        // The maximum sorting order that this can be rendered at.
        [SerializeField] 
        private int m_MaxSortingOrder;

        // The interval of time between which sparkles are rendered.
        [SerializeField] 
        private float m_SparkleRenderInterval = 0.075f;
        private float NudgedSparkleInterval => Random.Range(1f-NudgePercent, 1f+NudgePercent) * m_SparkleRenderInterval; 

        // The ticks until the next sparkle is rendered.
        [SerializeField, ReadOnly] 
        private float m_SparkleRenderTicks = 0f;

        // The amount of inconsistenty in the sparkle spawning.
        [SerializeField, Range(0f, 1f)] 
        private float m_NudgePercent = 0.25f;
        private float NudgePercent => m_NudgePercent > 1f ? 1f : m_NudgePercent < 0f ? 0f : m_NudgePercent;

        // The radius within which this sparkle can be spawned.
        [SerializeField] 
        private float m_SparkleSpawnRadius = 0.25f;
        
        // The speed at which the sparkle fades out.
        [SerializeField] 
        protected float m_FadeSpeed = 0.75f;  

        // The opacity threshold under which the particles are destroyed.
        [SerializeField] 
        protected float m_FadeThreshold = 0.1f;

        // The speed at which the particles rotate.
        [SerializeField] 
        protected float m_RotationSpeed = 5f;

        // Override the activation condition and play. 
        [SerializeField] 
        private bool m_OverrideAndPlay = false;

        #endregion

        void Start() {
            m_SparkleRenderTicks = m_SparkleRenderInterval;
        }       

        void FixedUpdate() {
            // Guard clause if the activation condition is not met.
            if (!ActivationCondition && !m_OverrideAndPlay) { Reset(); return; }

            // Check whether to spawn a new sparkle or not.
            SparkleSpawningSystem(Time.fixedDeltaTime);

            // Update the adjustments.
            UpdateSparkles(Time.fixedDeltaTime);
        }

        private void SparkleSpawningSystem(float dt) {
            bool finished = Timer.TickDown(ref m_SparkleRenderTicks, dt);
            if (finished) {
                SpawnSparkle();
                Timer.Start(ref m_SparkleRenderTicks, m_SparkleRenderInterval);
            }
        }

        private void SpawnSparkle() {
            string sparkleName = gameObject.name + " Particle " + m_Sparkles.Count.ToString();
            SpriteRenderer spriteRenderer = new GameObject(sparkleName, typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_SparkleSprite;
            spriteRenderer.transform.position = transform.position + (Vector3)Random.insideUnitCircle * m_SparkleSpawnRadius;
            spriteRenderer.sortingOrder = Random.Range(m_MinSortingOrder, m_MaxSortingOrder);
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.transform.localScale *= Random.Range(0.5f, 2f);
            m_Sparkles.Add(spriteRenderer);
        }

        protected virtual void UpdateSparkles(float dt) {
            SparkleAdjustments.FadeOut(ref m_Sparkles, 0.75f, 0.1f, dt);
            SparkleAdjustments.Rotate(ref m_Sparkles, 5f, dt);
        }

        protected virtual bool CheckActivationCondition() {
            return true;
        }

        public void Play() {
            gameObject.SetActive(true);
        }

        public void Reset() {
            SparkleAdjustments.Reset(ref m_Sparkles);
        }

        public void Stop() {
            SparkleAdjustments.Reset(ref m_Sparkles);
            gameObject.SetActive(false);
        }

        private void OnDestroy() {
            SparkleAdjustments.Reset(ref m_Sparkles);        }
        }
}
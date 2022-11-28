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

    public class Dust : MonoBehaviour {

        [SerializeField] private int m_MinSortingOrder;
        [SerializeField] private int m_MaxSortingOrder;

        [SerializeField] private Sprite m_Sprite;
        [SerializeField, ReadOnly] protected List<SpriteRenderer> m_Sparkles = new List<SpriteRenderer>();
        [SerializeField] protected float m_Ticks;
        [SerializeField] protected float m_Radius = 0.25f;

        [SerializeField] protected int m_Count = 8;   
        [SerializeField] protected float m_FadeSpeed = 0.15f;   
        [SerializeField] protected float m_RotationSpeed = 5f; 

        [SerializeField] protected float m_Speed = 0.15f;   
        [HideInInspector] private Vector3 m_SpawnPosition;

        public void Activate() {
            m_SpawnPosition = transform.position;
            for (int i = 0; i < m_Count; i++) {
                Spawn();
            }
        }

        void FixedUpdate() {
            m_Sparkles.RemoveAll(spark => spark == null);
            AdjustColor(Time.fixedDeltaTime);
            AdjustRotation(Time.fixedDeltaTime);
            AdjustPosition(Time.fixedDeltaTime);
            AdjustScale(Time.fixedDeltaTime);
        }

        private void AdjustColor(float deltaTime) {
            for (int i = 0; i < m_Sparkles.Count; i++) {
                Color c = m_Sparkles[i].color;
                c.a = c.a - m_FadeSpeed * deltaTime;
                if (c.a < 0.1f) {
                    Destroy(m_Sparkles[i].gameObject);
                }
                else {
                    m_Sparkles[i].color = c;
                }
            }
        }

        protected virtual void AdjustRotation(float deltaTime) {
            Vector3 deltaAngles = Vector3.forward * m_RotationSpeed * deltaTime;
            for (int i = 0; i < m_Sparkles.Count; i++) {
                m_Sparkles[i].transform.eulerAngles += deltaAngles;
            }
        }

        protected virtual void AdjustPosition(float deltaTime) {
            for (int i = 0; i < m_Sparkles.Count; i++) {
                Vector3 deltaPosition = (m_Sparkles[i].transform.position - m_SpawnPosition).normalized * m_Speed * deltaTime;
                m_Sparkles[i].transform.position += deltaPosition;
            }
        }

        protected virtual void AdjustScale(float deltaTime) {
            
        }

        private void Spawn() {
            SpriteRenderer spriteRenderer = new GameObject("Goo", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_Sprite;
            Vector3 direction = (Vector3)Random.insideUnitCircle;
            spriteRenderer.transform.position = transform.position + 0.5f * (direction + direction.normalized) * m_Radius;
            spriteRenderer.sortingOrder = Random.Range(m_MinSortingOrder, m_MaxSortingOrder);
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.transform.localScale *= Random.Range(0.5f, 2f);
            m_Sparkles.Add(spriteRenderer);
        }

        public void Reset() {
            m_Sparkles.RemoveAll(thing => thing == null);
            for (int i = 0; i < m_Sparkles.Count; i++) {
                Destroy(m_Sparkles[i].gameObject);
            }
            m_Sparkles.RemoveAll(thing => thing == null);
        }

        private void OnDestroy() {
            Reset();
        }

    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Sparkle : MonoBehaviour {

        [SerializeField] private Sprite m_Sprite;
        [SerializeField, ReadOnly] private List<SpriteRenderer> m_Sparkles = new List<SpriteRenderer>();
        [SerializeField] private float m_Ticks;
        [SerializeField] private float m_Interval = 0.075f;
        [SerializeField] private float m_Radius = 0.25f;

        [SerializeField] private float m_FadeSpeed = 0.75f;

        void Update() {
            bool leaveGoo = Timer.CountdownTicks(ref m_Ticks, m_Ticks == 0f, m_Interval, Time.deltaTime);
            LeaveGoo(leaveGoo);
            AdjustGooColor(Time.deltaTime);
        }

        private void AdjustGooColor(float deltaTime) {
            m_Sparkles.RemoveAll(thing => thing == null);
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

        private void LeaveGoo(bool leaveGoo) {
            if (!leaveGoo) {
                return;
            }
            SpriteRenderer spriteRenderer = new GameObject("Goo", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_Sprite;
            spriteRenderer.transform.position = transform.position + (Vector3)Random.insideUnitCircle * m_Radius;
            spriteRenderer.sortingLayerName = Screen.RenderingLayers.Foreground;
            spriteRenderer.sortingOrder = 3;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            m_Sparkles.Add(spriteRenderer);
        }

        public void Play() {
            gameObject.SetActive(true);
        }

        public void Stop() {
            gameObject.SetActive(false);
            Reset();
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
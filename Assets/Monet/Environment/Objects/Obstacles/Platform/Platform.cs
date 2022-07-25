/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteShapeController))]
    public class Platform : MonoBehaviour {

        [HideInInspector] protected BoxCollider2D m_Hitbox;
        [HideInInspector] protected SpriteShapeRenderer m_SpriteShapeRenderer;
        [HideInInspector] protected SpriteShapeController m_SpriteShapeController;

        [HideInInspector] protected Vector3 m_Origin;
        [HideInInspector] protected Vector3[] m_Path = null;
        [SerializeField, ReadOnly] protected int m_PathIndex;
        [SerializeField, ReadOnly] protected List<Transform> m_CollisionContainer = new List<Transform>();
        [SerializeField, ReadOnly] protected bool m_PressedDown;

        private static float PressedBuffer = 0.075f;
        [SerializeField, ReadOnly] private float m_PressedTicks;
        [SerializeField, ReadOnly] private bool m_OnPressedDown;

        public void Init(int length, Vector3[] path) {
            m_Origin = transform.position;
            m_Path = path;
            m_SpriteShapeController = GetComponent<SpriteShapeController>();
            m_SpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
            m_Hitbox = GetComponent<BoxCollider2D>();
            Obstacle.EditSpline(m_SpriteShapeController.spline, length);
            Obstacle.EditHitbox(m_Hitbox, length, 1f /16f);
            gameObject.layer = LayerMask.NameToLayer("Platform");
            
            Outline.Add(m_SpriteShapeRenderer, 0.5f, 16f);
            Outline.Set(m_SpriteShapeRenderer, Color.black);
        }

        protected virtual void Update() {
            bool wasPressed = m_PressedDown;
            Obstacle.PressedDown(transform.position, m_CollisionContainer, ref m_PressedDown);
            m_OnPressedDown = !wasPressed && m_PressedDown && m_PressedTicks == PressedBuffer ? true : m_OnPressedDown;

            Timer.UpdateTicks(ref m_PressedTicks, !m_OnPressedDown, PressedBuffer, Time.deltaTime);
            if (m_PressedTicks == 0f && m_OnPressedDown) {
                Obstacle.Shake(transform, m_Origin, 0f);
                Outline.Set(m_SpriteShapeRenderer, Color.black);
                m_OnPressedDown = false;
            }

            if (m_OnPressedDown) {
                Obstacle.Shake(transform, m_Origin, 0.0375f);
                Outline.Set(m_SpriteShapeRenderer, Color.white);
            }

        }

        private void OnCollisionEnter2D(Collision2D collision) {
            Obstacle.OnCollision(collision, ref m_CollisionContainer, true);
        }

        private void OnCollisionExit2D(Collision2D collision) {
            Obstacle.OnCollision(collision, ref m_CollisionContainer, false);
        }

    }
}
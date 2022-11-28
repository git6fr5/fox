/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using Platformer.Utilites;
using Platformer.Obstacles;
using Platformer.Rendering;
using Screen = Platformer.Rendering.Screen;

namespace Platformer.Obstacles {

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

        [SerializeField] private AudioClip OnPressedSound;

        public virtual void Init(int length, Vector3[] path) {
            m_Origin = transform.position;
            m_Path = path;
            m_SpriteShapeController = GetComponent<SpriteShapeController>();
            m_SpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();

            m_SpriteShapeRenderer.sortingLayerName = Screen.RenderingLayers.Foreground;
            m_SpriteShapeRenderer.color = Screen.ForegroundColorShift;

            m_Hitbox = GetComponent<BoxCollider2D>();
            Obstacle.EditSpline(m_SpriteShapeController.spline, length);
            Obstacle.EditHitbox(m_Hitbox, length, 5f /16f);
            gameObject.layer = LayerMask.NameToLayer("Platform");
        }

        protected virtual void Update() {
            bool waspressed = m_PressedDown;
            Obstacle.PressedDown(transform.position, m_CollisionContainer, ref m_PressedDown);
            bool isnowpressed = m_PressedDown;

            bool prevOnPressedDown = m_OnPressedDown;
            m_OnPressedDown = !waspressed && isnowpressed && m_PressedTicks == PressedBuffer ? true : m_OnPressedDown;

            if (!waspressed && isnowpressed) {
                SoundManager.PlaySound(OnPressedSound, 0.15f);
            }

            Timer.TriangleTickDownIf(ref m_PressedTicks, PressedBuffer, Time.deltaTime, !m_OnPressedDown);
            // if (m_PressedTicks == 0f && m_OnPressedDown) {
            //     Obstacle.Shake(transform, m_Origin, 0f);
            //     m_OnPressedDown = false;
            // }

            // if (m_OnPressedDown) {
            //     Obstacle.Shake(transform, m_Origin, 0.05f);
            // }

        }

        private void OnCollisionEnter2D(Collision2D collision) {
            Obstacle.OnCollision(collision, ref m_CollisionContainer, true);
        }

        private void OnCollisionExit2D(Collision2D collision) {
            Obstacle.OnCollision(collision, ref m_CollisionContainer, false);
        }

    }
}
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

        public void Init(int length, Vector3[] path) {
            m_Origin = transform.position;
            m_Path = path;
            m_SpriteShapeController = GetComponent<SpriteShapeController>();
            m_SpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
            m_Hitbox = GetComponent<BoxCollider2D>();
            Obstacle.EditSpline(m_SpriteShapeController.spline, length);
            Obstacle.EditHitbox(m_Hitbox, length, 1f /16f);
            gameObject.layer = LayerMask.NameToLayer("Platform");
        }

        private void Update() {
            Obstacle.PressedDown(transform.position, m_CollisionContainer, ref m_PressedDown);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            Obstacle.OnCollision(collision, ref m_CollisionContainer, true);
        }

        private void OnCollisionExit2D(Collision2D collision) {
            Obstacle.OnCollision(collision, ref m_CollisionContainer, false);
        }

    }
}
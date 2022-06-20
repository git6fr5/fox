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

        [SerializeField] protected int m_Length;
        [HideInInspector] private SpriteShapeController m_SpriteShape;
        [HideInInspector] private BoxCollider2D m_Hitbox;

        public void Init(int length) {
            m_Length = length;
            CreateSpline();
            CreateHitbox();
            gameObject.layer = LayerMask.NameToLayer("Platform");
        }

        private void CreateSpline() {
            m_SpriteShape = GetComponent<SpriteShapeController>();
            m_SpriteShape.spline.Clear();
            m_SpriteShape.spline.InsertPointAt(0, new Vector3(-0.5f, 0f, 0f));
            m_SpriteShape.spline.InsertPointAt(1, m_Length * Vector3.right + new Vector3(-0.5f, 0f, 0f));
            m_SpriteShape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            m_SpriteShape.spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        }

        private void CreateHitbox() {
            m_Hitbox = GetComponent<BoxCollider2D>();
            m_Hitbox.size = new Vector2(m_Length, 1f / 16f);
            m_Hitbox.offset = new Vector2((float)(m_Length - 1f) / 2f, 0.5f - 1f / 32f);
        }

    }
}
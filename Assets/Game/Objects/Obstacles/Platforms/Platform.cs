/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/* --- Definitions --- */
using LDtkTileData = LevelLoader.LDtkTileData;

///<summary>
///
///<summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteShapeController))]
public class Platform : MonoBehaviour {

    [SerializeField] protected int m_Length;
    [SerializeField] private LayerMask m_Mask;

    [HideInInspector] private SpriteShapeController m_SpriteShape;
    [HideInInspector] private BoxCollider2D m_Hitbox;

    public virtual void Init(int index, List<LDtkTileData> controlData) {
        Init();
    }

    public void Init() {

        m_Length = 0;
        bool continueSearch = true;
        List<Platform> cleanUp = new List<Platform>();
        while (continueSearch && m_Length < 50) {
            m_Length += 1;
            
            continueSearch = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + ((m_Length -1f) + 0.5f + GameRules.MovementPrecision) * Vector3.right, Vector2.right, 0.25f, m_Mask);
            if (hit.collider != null && hit.collider.GetComponent<Platform>()) {
                continueSearch = true;
                cleanUp.Add(hit.collider.GetComponent<Platform>());
            }

        }

        for (int i = 0; i < cleanUp.Count; i++) {
            Destroy(cleanUp[i].gameObject);
        }

        m_SpriteShape = GetComponent<SpriteShapeController>();
        m_SpriteShape.spline.Clear();
        m_SpriteShape.spline.InsertPointAt(0, new Vector3(-0.5f, 0f, 0f));
        m_SpriteShape.spline.InsertPointAt(1, m_Length * Vector3.right + new Vector3(-0.5f, 0f, 0f));
        m_SpriteShape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        m_SpriteShape.spline.SetTangentMode(1, ShapeTangentMode.Continuous);

        m_Hitbox = GetComponent<BoxCollider2D>();
        m_Hitbox.size = new Vector2(m_Length, 1f/16f);
        m_Hitbox.offset = new Vector2((float)(m_Length - 1f) / 2f, 0.5f - 1f/16f);
        
        gameObject.SetActive(true);
    }

}
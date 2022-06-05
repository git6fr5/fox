/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Spike : MonoBehaviour {

    [HideInInspector] protected SpriteRenderer m_SpriteRenderer;
    [HideInInspector] protected BoxCollider2D m_Hitbox;

    protected Vector2 m_Offset = new Vector2(0f, -0.25f);
    protected Vector2 m_Size = new Vector2(1f, 0.5f);
    [SerializeField] protected float m_Rotation = 0f;

    void Start() {
        Init();
    }

    public virtual void Init() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_Hitbox = GetComponent<BoxCollider2D>();
        m_Hitbox.size = m_Size;
        m_Hitbox.offset = m_Offset;
        m_Hitbox.isTrigger = true;

        gameObject.SetActive(true);
        transform.eulerAngles = new Vector3(0f, 0f, m_Rotation);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Controller temp = collider.GetComponent<Controller>();
        if (temp != null) {
            ProcessCollision(temp);
        }
    }
    
    private void ProcessCollision(Controller controller) {
        controller.Hurt(1);

        Vector2 direction = Quaternion.Euler(0f, 0f, m_Rotation) * Vector2.up;
        controller.Knockback(17.5f * direction.normalized, 0.075f);
    }

    public void Flip() {
        m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
        m_Hitbox.enabled = !m_Hitbox.enabled;
    }

}
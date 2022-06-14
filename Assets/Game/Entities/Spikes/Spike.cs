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
    [SerializeField, ReadOnly] protected bool m_Active = false;
    [SerializeField, ReadOnly] protected Vector3 m_Target;

    void Start() {
        Init();
    }

    protected virtual void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        // if ((transform.position - m_Target).sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
        //     transform.position = m_Target;
        //     return;
        // }
        transform.position += (m_Target - transform.position) * deltaTime * 1f / 0.15f;
    }

    public virtual void Init() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_Hitbox = GetComponent<BoxCollider2D>();
        m_Hitbox.size = m_Size;
        m_Hitbox.offset = m_Offset;
        m_Hitbox.isTrigger = true;

        gameObject.SetActive(true);
        transform.eulerAngles = new Vector3(0f, 0f, m_Rotation);

        m_Active = true;
        m_Target = transform.position;
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
        Vector3 direction = Quaternion.Euler(0f, 0f, m_Rotation) * Vector3.up;
        m_Target += (m_Active ? -1f : 1f) * direction;
        m_Active = !m_Active;
        m_SpriteRenderer.enabled = true;
        m_Hitbox.enabled = m_Active;

        // SoundController.PlaySound(flipSound, transform.position, 0.1f);
    }

    public AudioClip flipSound;

}
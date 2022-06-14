/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    [HideInInspector] private CircleCollider2D m_Hitbox;
    [HideInInspector] private Rigidbody2D m_Body;
    [HideInInspector] private SpriteRenderer m_SpriteRenderer;

    [SerializeField, ReadOnly] private int m_Value;
    [SerializeField, ReadOnly] private int m_Index;
    [SerializeField] private int[] m_Denominations;
    [SerializeField] private Sprite[] m_Sprites;

    public AudioClip pickUpSound;
    public AudioClip clinkSound;

    #endregion

    /* --- Unity --- */
    #region Unity

    void OnCollisionEnter2D(Collision2D collision) {
        Player player = collision.collider.GetComponent<Player>();
        State state = player?.GetComponent<Controller>()?.State;
        if (player != null && state != null) {
            state.AddGold(m_Value);
            // Play sound
            // Play deattached effect
            SoundController.PlaySound(pickUpSound, transform.position);
            Destroy(gameObject);
        }

        if (collision.collider.gameObject.layer == GameRules.GroundCollisionLayer) {
            SoundController.PlaySound(clinkSound, transform.position, 0.1f);
             // Play effect
        }
    }
    
    #endregion

    /* --- Initialization --- */
    #region Initialization

    // public static void Drop(int value) {
    //     GameRules.MainCoin.Drop(value);
    // }

    public void Drop(int value, Vector3 position) {
        int depth = 0;
        while (value > 0 && depth < 100) {
            for (int i = 0; i < m_Denominations.Length; i++) {

                if (value >= m_Denominations[i]) {
                    float prob = UnityEngine.Random.Range(0f, 1f);
                    if (prob > 0.5f) {
                        Create(i, position);
                        value -= m_Denominations[i];
                    }
                }

            }
            depth += 1;
        }
            
    }

    public void Create(int index, Vector3 position) {
        Coin coin = Instantiate(gameObject, position, Quaternion.identity, null).GetComponent<Coin>();
        coin.Init(index);
    }

    public void Init(int index) {
        m_Hitbox = GetComponent<CircleCollider2D>();
        // m_Hitbox.isTrigger = true;

        m_Body = GetComponent<Rigidbody2D>();
        
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = m_Sprites[index];
        m_Value = m_Denominations[index];

        StartCoroutine(IEDespawn(8f));

        gameObject.SetActive(true);
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        direction.y *= Mathf.Sign(direction.y);
        m_Body.velocity = direction * UnityEngine.Random.Range(5f, 15f);

        // Play sound
        // Play effect

    }

    private IEnumerator IEDespawn(float duration) {
        yield return new WaitForSeconds(duration);
        if (gameObject != null && m_SpriteRenderer != null) {
            for (int i = 0; i < 5; i++) {
                m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        if (gameObject != null) {
            Destroy(gameObject);
        }
    }

    #endregion

}
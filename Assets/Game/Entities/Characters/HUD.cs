/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///<summary>
[DefaultExecutionOrder(1000)]
public class HUD : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    // Controller.
    [HideInInspector] private Controller m_Controller;
    private State m_State => m_Controller.State;

    // Cooldown.
    [SerializeField] private SpriteRenderer m_Cooldown;
    
    // Healthbar.
    private bool HealthChanged => m_MaxHearts != m_State.MaxHealth || m_Hearts != m_State.Health;
    [SerializeField] private List<SpriteRenderer> m_Healthbar = new List<SpriteRenderer>();
    [SerializeField, ReadOnly] private bool m_ShowingHealthbar;
    [SerializeField, ReadOnly] private float m_HealthbarTicks;
    [SerializeField] private float m_HealthbarFadeDuration;
    [SerializeField] private float m_HealthbarShowDuration;
    [SerializeField, ReadOnly] private int m_MaxHearts;
    [SerializeField, ReadOnly] private int m_Hearts;
    [SerializeField] private Sprite m_HeartSprite;
    [SerializeField] private Sprite m_EmptyHeartSprite;

    // Flashing hearts.
    [SerializeField, ReadOnly] private int m_FlashingHearts;
    [SerializeField, ReadOnly] private bool m_Flash = false;
    [SerializeField, ReadOnly] private float m_FlashTicks = 0f;
    [SerializeField] private float m_FlashInterval = 0.2f;
    [SerializeField] private float m_FlashDuration;

    #endregion

    /* --- Unity --- */
    #region Unity
    
    void Start() {
        m_Controller = transform.parent.GetComponent<Controller>();
    }
    
    void Update() {
        transform.eulerAngles = Vector3.zero;
        UpdateHealthbar();
        UpdateCooldown();
    }

    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        if (m_ShowingHealthbar) {
            m_HealthbarTicks += deltaTime;
        }

        m_FlashTicks += deltaTime;
        if (m_FlashTicks > m_FlashInterval) {
            m_Flash = !m_Flash;
            m_FlashTicks -= m_FlashInterval;
        }

    }
    
    #endregion

    /* --- Healthbar --- */
    #region Healthbar
    
    private void UpdateHealthbar() {
        if (HealthChanged) {
            ShowHealthbar();
        }

        if (m_ShowingHealthbar) {
            RenderHealthbar();
        }
    }

    private void RenderHealthbar() {
        if (m_HealthbarTicks > m_HealthbarShowDuration + m_HealthbarFadeDuration) {
            m_ShowingHealthbar = false;
        }

        float alpha = m_HealthbarTicks < m_HealthbarShowDuration ? 1f : 1f - ((m_HealthbarTicks - m_HealthbarShowDuration) / m_HealthbarFadeDuration);
        for (int i = 0; i < m_Healthbar.Count; i++) {
            m_Healthbar[i].color = new Color(1f, 1f, 1f, alpha / 2f);
        }
    }

    private void ShowHealthbar() {
        m_FlashingHearts = m_Hearts;
        SetHealthbar();

        m_HealthbarTicks = 0f;
        m_ShowingHealthbar = true;
    }

    private void SetHealthbar() {
        // Update the max hearts.
        m_MaxHearts = m_State.MaxHealth;
        if (m_Healthbar.Count < m_MaxHearts) {
            for (int i = m_Healthbar.Count; i < m_MaxHearts; i++) {
                AddHeart(i);
            }
        }

        // Update the hearts.
        m_Hearts = m_State.Health;
        for (int i = 0; i < m_Hearts; i++) {
            m_Healthbar[i].sprite = m_HeartSprite;
        }
        for (int i = m_Hearts; i < m_MaxHearts; i++) {
            m_Healthbar[i].sprite = m_EmptyHeartSprite;
        }
    }

    private void AddHeart(int i) {
        SpriteRenderer renderer = new GameObject("Heart", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        renderer.transform.SetParent(transform);

        float x = (i - ((float)m_MaxHearts - 1f) / 2f) * 0.45f;
        renderer.transform.localPosition = new Vector3(x, -1f, 0f);
        renderer.sortingLayerName = GameRules.UIRenderingLayer;
        m_Healthbar.Add(renderer);
    }

    
    #endregion

    /* --- Cooldown --- */
    #region Cooldown

    private void UpdateCooldown() {
        if (m_State.Projectile.CanFire) {
            m_Cooldown.gameObject.SetActive(false);
            return;
        }

        m_Cooldown.gameObject.SetActive(true);
        float ratio = m_State.Projectile.Ticks / m_State.Projectile.Cooldown;
        float offset = 1f - (m_State.Projectile.Ticks / m_State.Projectile.Cooldown);
        // m_Cooldown.material.SetFloat("_RatioX", ratio);
        m_Cooldown.transform.localScale = new Vector3(ratio, 1f, 1f);
        m_Cooldown.transform.localPosition = new Vector3(-offset / 2f, 0.75f, 1f);
        m_Cooldown.color = new Color(1f, 1f, 1f, 0.5f);
    }

    #endregion

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Screen.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class Screen : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Instance.
    public static Screen Instance;
    public static Camera MainCamera => Instance.m_MainCamera;
    public static Vector2 MousePosition => (Vector2)Instance.m_MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

    // Components.
    [HideInInspector] public Camera m_MainCamera;
    [SerializeField] public PixelPerfectCamera m_PixelPerfectCamera;

    // Settings.
    [SerializeField, ReadOnly] private Vector3 m_Origin;
    [SerializeField, ReadOnly] private Vector2 m_ScreenSize;
    public static Vector2 ScreenSize => Instance.m_ScreenSize;


    [Header("Shake")]
    [SerializeField] private AnimationCurve m_Curve;
    [SerializeField, ReadOnly] public float m_ShakeStrength = 1f;
    [SerializeField, ReadOnly] public float m_ShakeDuration = 0.5f;
    [SerializeField, ReadOnly] float m_ElapsedTime = 0f;
    [SerializeField, ReadOnly] public bool m_Shake;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        transform.position = m_Origin;
        if (m_Shake) {
            m_Shake = Shake();
        }
        Follow();
        SetProfile();
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    // Initializes this script.
    public void Init() {
        m_MainCamera = GetComponent<Camera>();
        m_Origin = transform.position;
        m_ScreenSize = new Vector2(m_PixelPerfectCamera.refResolutionX, m_PixelPerfectCamera.refResolutionY) / m_PixelPerfectCamera.assetsPPU;
        Instance = this;
    }

    #endregion

    /* --- Shaking --- */
    #region Shaking

    public bool Shake() {
        m_ElapsedTime += Time.deltaTime;
        if (m_ElapsedTime >= m_ShakeDuration) {
            m_ElapsedTime = 0f;
            return false;
        }
        float strength = m_ShakeStrength * m_Curve.Evaluate(m_ElapsedTime / m_ShakeDuration);
        transform.position += (Vector3)Random.insideUnitCircle * strength;
        return true;
    }

    public static void CameraShake(float strength, float duration) {
        if (strength == 0f) {
            return;
        }
        if (!Instance.m_Shake) {
            Instance.m_ShakeStrength = strength;
            Instance.m_ShakeDuration = duration;
            Instance.m_Shake = true;
        }
        else {
            Instance.m_ShakeStrength = Mathf.Max(Instance.m_ShakeStrength, strength);
            Instance.m_ShakeDuration = Mathf.Max(Instance.m_ShakeDuration, Instance.m_ElapsedTime + duration);
        }
    }

    #endregion

    /* --- Settings --- */
    #region Settings

    private void Follow() {
        if (GameRules.MainPlayer == null) { return; }

        Level level = GameRules.MainPlayer.level;
        
        if (true) {
            transform.position = new Vector3(GameRules.MainPlayer.transform.position.x, GameRules.MainPlayer.transform.position.y + 4f, transform.position.z);
            m_Origin = transform.position;
        }
        else {
            int maxW = 40; int boundW = 23;
            int maxH = 26; int boundH = 15;

            float width = level.Width >= maxW ? boundW : (float)GameRules.MainPlayer.level.Width;
            float height = level.Height >= maxH ? boundH : (float)GameRules.MainPlayer.level.Height;
            
            float x = level.Width >= maxW ? GameRules.MainPlayer.transform.position.x : (float)GameRules.MainPlayer.level.Center.x;
            float y = level.Height >= maxH ? GameRules.MainPlayer.transform.position.y : (float)GameRules.MainPlayer.level.Center.y;

            bool left = x + width / 2f > (float)GameRules.MainPlayer.level.Center.x + level.Width / 2f;
            bool right = x - width / 2f < (float)GameRules.MainPlayer.level.Center.x - level.Width / 2f;
            if (left) {
                x = (float)GameRules.MainPlayer.level.Center.x + (level.Width - width) / 2f;
            }
            else if (right) {
                x = (float)GameRules.MainPlayer.level.Center.x - (level.Width - width) / 2f;
            }

            bool up = y + height / 2f > (float)GameRules.MainPlayer.level.Center.y + level.Height / 2f;
            bool down = y - height / 2f < (float)GameRules.MainPlayer.level.Center.y - level.Height / 2f;
            if (up) {
                y = (float)GameRules.MainPlayer.level.Center.y + (level.Height - height) / 2f;
            }
            else if (down) {
                y = (float)GameRules.MainPlayer.level.Center.y - (level.Height - height) / 2f;
            }

            transform.position = new Vector3(x, y, transform.position.z);
            m_PixelPerfectCamera.refResolutionX = m_PixelPerfectCamera.assetsPPU * (int)width;
            m_PixelPerfectCamera.refResolutionY = m_PixelPerfectCamera.assetsPPU * (int)height;
            
            m_Origin = transform.position;
            m_ScreenSize = new Vector2(m_PixelPerfectCamera.refResolutionX, m_PixelPerfectCamera.refResolutionY) / m_PixelPerfectCamera.assetsPPU;
        }
    }

    private void SetProfile() {
        //
    }

    #endregion

    /* --- Debugging --- */
    #region Debugging

    void OnDrawGizmos() {
        Vector3 screenSize = new Vector3((float)m_PixelPerfectCamera.refResolutionX / m_PixelPerfectCamera.assetsPPU, (float)m_PixelPerfectCamera.refResolutionY / m_PixelPerfectCamera.assetsPPU, 0f);
        Gizmos.DrawWireCube(transform.position, screenSize);
        this.m_ScreenSize = screenSize;
    }

    #endregion

}

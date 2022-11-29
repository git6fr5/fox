// TODO: Clean

/* --- Libraries --- */
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using UnityEngine.U2D;

using Platformer.Obstacles;
using Platformer.Rendering;
using Platformer.Utilites;

using PixelPerfectCamera = UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

namespace Platformer.Rendering {

    /// <summary>
    /// Controls basic rendering functionality.
    /// </summary>
    [DefaultExecutionOrder(-900)]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(PixelPerfectCamera))]
    public class Screen : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        public string ClassName => GetType().Name;
        public string MethodName => MethodBase.GetCurrentMethod().Name;

        // Instance.
        public static Screen Instance;
        public static Camera MainCamera => Instance.m_MainCamera;
        public static Vector2 MousePosition => (Vector2)Instance.m_MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

        // Components.
        [HideInInspector] public Camera m_MainCamera;
        [SerializeField] public PixelPerfectCamera m_PixelPerfectCamera;
        public static int PixelSize => Instance.m_PixelPerfectCamera.assetsPPU;

        // Settings.
        [SerializeField, ReadOnly] private Vector3 m_Origin;
        [SerializeField, ReadOnly] private Vector2 m_ScreenSize;
        public static Vector2 ScreenSize => Instance.m_ScreenSize;

        [Header("Following")]
        [SerializeField] private Transform m_Follow;
        [SerializeField] private float m_FollowBounds = 0.25f;
        private float FollowBounds => m_FollowBounds;
        [SerializeField] private bool m_FollowAbsolute;

        [Header("Snap")]
        [SerializeField] private bool m_Snap;
        public static bool IsSnapping => Instance.m_Snap;
        [SerializeField] private Vector2 m_SnapPosition;
        [SerializeField] private float m_SnapSpeed;

        // Rendering. // This should be in screen
        [SerializeField] private RenderingLayer m_RenderingLayers;
        public static RenderingLayer RenderingLayers => Instance.m_RenderingLayers;

        // Frame Rate.
        [SerializeField] private float m_FrameRate = 6f;
        public static float FrameRate => Instance.m_FrameRate;

        // Shaking.
        [SerializeField] private AnimationCurve m_Curve;
        [SerializeField, ReadOnly] public float m_ShakeStrength = 1f;
        [SerializeField, ReadOnly] public float m_ShakeDuration = 0.5f;
        [SerializeField, ReadOnly] float m_ElapsedTime = 0f;
        [SerializeField, ReadOnly] public bool m_Shake;

        [SerializeField] private Volume[] m_Lighting;
        [SerializeField] private VisualEffect[] m_Weather;

        // Color shifts.
        [SerializeField] private Color m_ForegroundColorShift;
        public static Color ForegroundColorShift => Instance.m_ForegroundColorShift;

        #endregion

        // Runs once before the first frame.
        void Awake() {
            Instance = this;
            Game.OnGameLoad(0);
        }

        // Initializes this script.
        public void OnGameLoad(int loadPhase) {
            Game.Log(ClassName, MethodName, "Game Phase " + loadPhase.ToString());
            if (loadPhase == 0) {
                gameObject.SetActive(false);
            }
            if (loadPhase == 1) {
                m_MainCamera = GetComponent<Camera>();
                m_Origin = transform.position;

                float x = m_PixelPerfectCamera.refResolutionX;
                float y = m_PixelPerfectCamera.refResolutionY;
                float ppu = m_PixelPerfectCamera.assetsPPU;
                m_ScreenSize = new Vector2(x, y) / ppu;
            }
        }

        void LateUpdate() {
            transform.position = m_Origin;
            if (m_Follow != null) {
                m_Origin = Follow(m_Follow, transform, m_FollowBounds, m_FollowAbsolute);
            }
            if (m_Snap) {
                m_Origin = Snap(m_SnapPosition, transform, m_SnapSpeed, ref m_Snap, Game.Physics.MovementPrecision);
            }
            if (m_Shake) {
                m_Shake = Shake();
            }

        }

        public void Shape(Vector2Int shape, int ppu = 64) {
            // ppu /= 2;
            m_PixelPerfectCamera.refResolutionX = shape.x * ppu;
            m_PixelPerfectCamera.refResolutionY = shape.y * ppu;
            m_PixelPerfectCamera.assetsPPU = ppu;
            m_ScreenSize = new Vector2(m_PixelPerfectCamera.refResolutionX, m_PixelPerfectCamera.refResolutionY) / m_PixelPerfectCamera.assetsPPU;
        }

        public void Snap(Vector2 targetPosition, float snapTime = 0.33f) {
            m_Snap = true;
            m_SnapPosition = targetPosition;
            float distance = (targetPosition - (Vector2)transform.position).magnitude;
            m_SnapSpeed = distance / snapTime;
        }

        public static Vector3 Snap(Vector2 targetPosition, Transform cameraTransform, float snapSpeed, ref bool continueSnap, float bounds = 0f) {
            Vector3 actualTarget = (Vector3)targetPosition + Vector3.forward * cameraTransform.position.z;
            Obstacle.Move(cameraTransform, actualTarget, snapSpeed, Time.deltaTime, null);
            
            if (cameraTransform.position == actualTarget) {
                continueSnap = false;
            }
            return cameraTransform.position;
        }

        private static Vector3 Follow(Transform followTransform, Transform cameraTransform, float bounds, bool absolute) {
            Vector2 targetPosition = (Vector2)followTransform.position;
            Vector2 deltaPosition = targetPosition - (Vector2)cameraTransform.position;
            
            if (deltaPosition.magnitude > bounds) {
                if (absolute) {
                    cameraTransform.position = new Vector3(targetPosition.x, targetPosition.y, cameraTransform.position.z);
                }
                else {
                    deltaPosition = deltaPosition.normalized * Mathf.Max(1f, deltaPosition.magnitude);
                    cameraTransform.position += (Vector3)deltaPosition * 2f * Time.deltaTime;
                }
            }

            return cameraTransform.position;
        }

        public static void Shake(float strength, float duration) {
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

        public static Vector2 RandomPositionWithinBounds() {
            float x = Instance.m_ScreenSize.x;
            float y = Instance.m_ScreenSize.y;
            return (Vector2)Instance.transform.position + new Vector2(Random.Range(-x, x), Random.Range(-y, y)) * 1.5f;
        }

    }

}

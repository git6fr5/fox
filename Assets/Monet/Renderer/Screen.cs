/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Monet;

namespace Monet {

    /// <summary>
    /// Controls basic rendering functionality.
    /// </summary>
    [DefaultExecutionOrder(-900)]
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

        [Header("Following")]
        [SerializeField] private Transform m_Follow;
        [SerializeField] private float m_FollowBounds = 0.25f;
        private float FollowBounds => m_FollowBounds;

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

        #endregion

        // Runs once before the first frame.
        void Awake() {
            Instance = this;
        }

        void Start() {
            Init();
        }

        // Initializes this script.
        public void Init() {
            m_MainCamera = GetComponent<Camera>();
            m_Origin = transform.position;
            m_ScreenSize = new Vector2(m_PixelPerfectCamera.refResolutionX, m_PixelPerfectCamera.refResolutionY) / m_PixelPerfectCamera.assetsPPU;
        }

        void LateUpdate() {
            transform.position = m_Origin;
            if (m_Follow != null) {
                m_Origin = Follow(m_Follow, transform, m_FollowBounds);
            }
            if (m_Snap) {
                m_Origin = Snap(m_SnapPosition, transform, m_SnapSpeed, ref m_Snap, Game.Physics.MovementPrecision);
            }
        }

        public void Shape(Vector2Int shape, int ppu = 16) {
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
            Vector2 displacement = targetPosition - (Vector2)cameraTransform.position;
            
            if (displacement.magnitude > bounds) {
                Vector2 deltaPosition = displacement.normalized * snapSpeed * Time.deltaTime;
                if (displacement.magnitude < deltaPosition.magnitude) {
                    cameraTransform.position = new Vector3(targetPosition.x, targetPosition.y, cameraTransform.position.z);
                    continueSnap = false;
                }
                else {
                    cameraTransform.position += (Vector3)deltaPosition;
                }
            }
            return cameraTransform.position;
        }

        private static Vector3 Follow(Transform followTransform, Transform cameraTransform, float bounds) {
            Vector2 targetPosition = (Vector2)followTransform.position;
            Vector2 deltaPosition = targetPosition - (Vector2)cameraTransform.position;
            
            if (deltaPosition.magnitude > bounds) {
                deltaPosition = deltaPosition.normalized * Mathf.Max(1f, deltaPosition.magnitude);
                cameraTransform.position += (Vector3)deltaPosition * 2f * Time.deltaTime;
            }
            return cameraTransform.position;
        }

    }

}

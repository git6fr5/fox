/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Monet.UI;
using UnityEngine.SceneManagement;

namespace Monet.UI {

    public class Cutscene : MonoBehaviour {

        [HideInInspector] private Vector3 m_CameraOrigin;

        [SerializeField] private AnimationCurve m_FlowCurve;

        [SerializeField] private float m_Duration = 10f;
        [SerializeField, ReadOnly] private float m_Ticks = 0f;
        public float Ratio => m_FlowCurve.Evaluate(m_Ticks / m_Duration);
        public float InverseRatio => m_Duration / (m_Duration + m_Ticks);

        [SerializeField] private float m_Rotation = 720f;
        [SerializeField] private Vector3 m_RotationAxis;
        public float RotationRatio => m_Rotation * Ratio;
        public Vector3 DeltaRotation => RotationRatio * m_RotationAxis;

        [SerializeField] private float m_Shake = 0.125f;
        public float ShakeRatio => m_Shake * Ratio;

        [SerializeField] private VisualEffect[] m_ScaledVFX;
        [SerializeField] private Transform m_Shield;

        [SerializeField] private Poem m_Poem;

        [SerializeField] private bool m_LoadZone;
        [SerializeField] private Zone m_Zone;

        void Start() {
            m_Ticks = 0f;
            for (int i = 0; i < m_ScaledVFX.Length; i++) {
                m_ScaledVFX[i].SetFloat("Ratio", 0f);
            }
            m_CameraOrigin = Camera.main.transform.position;
            m_Poem.SetRatio(InverseRatio);
            m_Poem.Play();
        }

        void Update() {
            Obstacle.Shake(Camera.main.transform, m_CameraOrigin, ShakeRatio);
            Obstacle.Shake(transform, Vector3.zero, ShakeRatio);
            for (int i = 0; i < m_ScaledVFX.Length; i++) {
                m_ScaledVFX[i].SetFloat("Ratio", Ratio);
            }
            m_Poem.SetRatio(InverseRatio);
        }

        void FixedUpdate() {
            bool finished = Timer.TickUp(ref m_Ticks, m_Duration, Time.fixedDeltaTime);
            m_Shield.transform.eulerAngles += DeltaRotation * Time.fixedDeltaTime;

            if (finished) {
                if (m_LoadZone) {
                    SceneManager.LoadScene(Zones.Get(m_Zone));
                }
            }

        }

    }

}
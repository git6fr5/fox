/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Boomerang : Projectile {

        // The fastest way to kill the fun is to have realistic physics.
        // [SerializeField] private float m_ThrowDistance;
        // [SerializeField] private float m_CurveRadius;

        // [SerializeField, ReadOnly] private float m_Angle;
        // [SerializeField, ReadOnly] private Vector2 m_Target;
        // [SerializeField, ReadOnly] private float m_PreviousDistance = Mathf.Infinity;
        // [SerializeField, ReadOnly] private float m_CurveDirection = 1f;
        // [SerializeField, ReadOnly] private bool m_Curve = false;
        // [SerializeField, ReadOnly] private bool m_EndCurve = false;
        // [SerializeField, ReadOnly] private float m_PreviousAlignment = Mathf.Infinity;

        // public override void Fire(bool fire, Vector2 direction, List<string> targets) {
        //     direction = Quaternion.Euler(0f, 0f, -45f) * direction;
        //     base.Fire(fire, direction, targets);
        // }

        // void FixedUpdate() {
        //     m_Body.velocity += (Vector2)(Quaternion.Euler(0f, 0f, 90f) * m_Body.velocity.normalized * m_Boomeranginess * Time.fixedDeltaTime);
        // }

        // public override void Fire(bool fire, Vector2 direction, List<string> targets) {
            
        //     // sin(o/h) = angle;
        //     float halfangle = 180f * Mathf.Sin(m_CurveRadius / m_ThrowDistance) / Mathf.PI;
        //     float angle = halfangle * 2f;

        //     // Since we always want to rotate it upwards.
        //     Vector2 adjustedDirection = Vector2.zero;
        //     Vector2 target = Vector2.zero;
        //     float curveDirection = 0f;
        //     if (direction.x < 0) {
        //         // CW
        //         adjustedDirection = Quaternion.Euler(0f, 0f, -angle) * direction;
        //         target = Quaternion.Euler(0f, 0f, -halfangle) * direction;
        //         curveDirection = -1f;
        //     }
        //     else {
        //         // CCW
        //         adjustedDirection = Quaternion.Euler(0f, 0f, angle) * direction;
        //         target = Quaternion.Euler(0f, 0f, halfangle) * direction;
        //         curveDirection = 1f;
        //     }

        //     m_Angle = angle;
        //     m_Target = transform.position + (Vector3)(target.normalized * m_ThrowDistance);
        //     m_CurveDirection = curveDirection;
        //     m_Curve = false;
        //     m_EndCurve = false;
        //     base.Fire(fire, adjustedDirection, targets);
        // }


        // void FixedUpdate() {

        //     if (!m_Curve) {
        //         // Distance to the node we rotate around.
        //         float distance = (m_Target - (Vector2)transform.position).magnitude;
        //         if (m_PreviousDistance < distance) {
        //             m_Curve = true;
        //         }
        //         else {
        //             m_PreviousDistance = distance;
        //         }
        //     }

        //     if (m_Curve && !m_EndCurve) {

        //         Vector2 direction = (m_Target - (Vector2)transform.position).normalized;
        //         Vector2 unitVelocity = Quaternion.Euler(0f, 0f, m_CurveDirection * 90f) * direction;
                
        //         float alignment = (unitVelocity - (-m_CurveDirection) * Vector2.right).magnitude;
        //         if (alignment > m_PreviousAlignment) {
        //             m_Body.velocity = m_Body.velocity.magnitude * (-m_CurveDirection) * Vector2.right;
        //             m_EndCurve = true;
        //         }
        //         else {
        //             m_Body.velocity = m_Body.velocity.magnitude * unitVelocity;
        //             m_PreviousAlignment = alignment;
        //         }
                
        //     }

        // }

        // [SerializeField, ReadOnly] private Transform m_Magnet;
        // [SerializeField] private float m_Rotation = 5f;

        // public override void Fire(bool fire, Vector2 direction, List<string> targets) {
        //     m_Magnet = transform.parent;
        //     if (direction.x < 0) {
        //         direction = Quaternion.Euler(0f, 0f, -60f) * direction;
        //     }
        //     else {
        //         direction = Quaternion.Euler(0f, 0f, 60f) * direction;
        //     }
        //     base.Fire(fire, direction, targets);
        // }

        // float cachedRotationDirection;

        // void FixedUpdate() {
        //     float deltaTime = Time.fixedDeltaTime;

        //     Vector2 currVelocity = m_Body.velocity;
        //     Vector2 direction = (m_Magnet.position - transform.position).normalized;

        //     float rotationDirection = -1f * (Mathf.Sign(currVelocity.x) * Mathf.Sign(currVelocity.y));
        //     if (rotationDirection == 0f) {
        //         rotationDirection = cachedRotationDirection;
        //     }
        //     cachedRotationDirection = rotationDirection;

        //     Quaternion quat = Quaternion.Euler(0f, 0f, rotationDirection * m_Rotation * deltaTime);
        //     Vector2 newVelocity = (Vector2)(quat * currVelocity);

        //     m_Body.velocity = newVelocity;
        // }

        [SerializeField, ReadOnly] private Transform m_Magnet;
        [SerializeField] private float m_MagnetStrength;
        [SerializeField] private float m_MagnetDelay;
        [SerializeField, ReadOnly] private float m_MagnetTicks = 0f;

        public override void Fire(Input input, bool fire, Vector2 direction, List<string> targets) {
            m_Magnet = transform.parent;
            Timer.CountdownTicks(ref m_MagnetTicks, true, m_MagnetDelay, 0f);
            base.Fire(input, fire, direction, targets);
        }

        void FixedUpdate() {
            float deltaTime = Time.fixedDeltaTime;
            Timer.CountdownTicks(ref m_MagnetTicks, false, m_MagnetDelay, deltaTime);
            if (m_MagnetTicks == 0f) {
                Vector2 direction = (m_Magnet.position - transform.position).normalized;
                m_Body.velocity += m_MagnetStrength * direction * deltaTime;
            }
            if (m_Body.velocity.magnitude > m_Speed) {
                m_Body.velocity = m_Body.velocity.normalized * m_Speed;
            }
        }

    }
}
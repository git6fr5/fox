// TODO: Clean
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Decor;

namespace Platformer.Decor {

    ///<summary>
    ///
    ///<summary>
    public class HangingRope : Rope {

        [SerializeField, Range(0f, 100f)] private float m_RopeLengthVariationPercent = 30f;
        [SerializeField, Range(0f, 100f)] private float m_RopeEndWidthPercent = 30f;
        [SerializeField, Range(0f, 100f)] private float m_RopeEndWidthVariationPercent = 30f;

        protected override void OnAwake() {
            edgeCollider.isTrigger = true;

            float endVar = Random.Range(-m_RopeEndWidthVariationPercent, m_RopeEndWidthVariationPercent);
            float endWidthPerc = (m_RopeEndWidthPercent + endVar) / 100f;
            lineRenderer.endWidth = ropeWidth * endWidthPerc;
            edgeCollider.edgeRadius = ropeWidth;

            transform.position += Vector3.right * Random.Range(-0.35f, 0.35f);

            ropeLength += ropeLength * Random.Range(-m_RopeLengthVariationPercent, m_RopeLengthVariationPercent) / 100f;
        }

        protected override void Constraints() {
            ropeSegments[0] = Vector3.zero;
            for (int i = 1; i < segmentCount; i++) {
                // Get the distance and direction between the segments.
                float newDist = (ropeSegments[i - 1] - ropeSegments[i]).magnitude;
                Vector3 direction = (ropeSegments[i - 1] - ropeSegments[i]).normalized;

                // Get the error term.
                float error = newDist - SegmentLength;
                Vector3 errorVector = direction * error;

                // Adjust the segments by the error term.
                if (i != 1) {
                    ropeSegments[i - 1] -= errorVector * 0.5f;
                }
                ropeSegments[i] += errorVector * 0.5f;
            }
        }

    }
}
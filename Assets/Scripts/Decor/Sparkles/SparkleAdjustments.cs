/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

namespace Platformer {

    ///<summary>
    ///
    ///<summary>
    public static class SparkleAdjustments {

        // Resets the particles.
        public static void Reset(ref List<SpriteRenderer> sparkles) {
            sparkles.RemoveAll(sparkle => sparkle == null);
            for (int i = 0; i < sparkles.Count; i++) {
                UnityEngine.MonoBehaviour.Destroy(sparkles[i].gameObject);
            }
            sparkles = new List<SpriteRenderer>();

        }

        // Fades the particle out.
        public static void FadeOut(ref List<SpriteRenderer> sparkles, float fadeSpeed, float fadeThreshold, float dt) {
            sparkles.RemoveAll(sparkle => sparkle == null);
            for (int i = 0; i < sparkles.Count; i++) {
                Color cacheColor = sparkles[i].color;
                cacheColor.a = cacheColor.a - fadeSpeed * dt;
                if (cacheColor.a < fadeThreshold) {
                    UnityEngine.MonoBehaviour.Destroy(sparkles[i].gameObject);
                }
                else {
                    sparkles[i].color = cacheColor;
                }
            }
        }

        // Rotates the particle.
        public static void Rotate(ref List<SpriteRenderer> sparkles, float rotationSpeed, float dt) {
            sparkles.RemoveAll(sparkle => sparkle == null);
            Vector3 deltaAngles = Vector3.forward * rotationSpeed * dt;
            for (int i = 0; i < sparkles.Count; i++) {
                sparkles[i].transform.eulerAngles += deltaAngles;
            }
        }

        // Moves the particle in a specific direction.
        public static void Move(ref List<SpriteRenderer> sparkles, Vector3 direction, float speed, float dt) {
            sparkles.RemoveAll(sparkle => sparkle == null);
            Vector3 dx = direction * speed * dt;
            for (int i = 0; i < sparkles.Count; i++) {
                sparkles[i].transform.position += dx;
            }
        }

        // Moves according to opacity on 1 axis and with regular movement on the other axis.
        public static void OpacityMove(ref List<SpriteRenderer> sparkles, Vector3 opacityDir, float opacitySpeed, Vector3 direction, float speed, float dt) {
            sparkles.RemoveAll(sparkle => sparkle == null);
            Vector3 dx_0 = direction * speed * dt;
            for (int i = 0; i < sparkles.Count; i++) {
                Vector3 dx_1 = opacityDir * opacitySpeed * sparkles[i].GetComponent<SpriteRenderer>().color.a * dt;
                sparkles[i].transform.position += (dx_0 + dx_1);
            }
        }

        // Moves according to opacity in 2 different directions, with 2 different speeds.
        public static void OpacityMove2D(ref List<SpriteRenderer> sparkles, Vector3 opacityDirA, float opacitySpeedA, Vector3 opacityDirB, float opacitySpeedB, float dt) {
            sparkles.RemoveAll(sparkle => sparkle == null);
            for (int i = 0; i < sparkles.Count; i++) {
                Vector3 dx_1 = sparkle * opacitySpeedA * sparkles[i].GetComponent<SpriteRenderer>().color.a * dt;
                Vector3 dx_0 = opacityDirB * opacitySpeedB * sparkles[i].GetComponent<SpriteRenderer>().color.a * dt;
                sparkles[i].transform.position += (dx_0 + dx_1);
            }
        }

    }
}
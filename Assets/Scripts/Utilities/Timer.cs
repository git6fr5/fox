using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilites;

namespace Platformer.Utilites {

    public class Timer {

        // Starts the timer to its max value.
        public static void Start(ref float t, float T) {
            t = T; 
        }

        // Starts the timer to its max value if the predicate is fulfilled.
        public static void StartIf(ref float t, float T, bool p) {
            if (p) {  t = T;  }
        }

        // Stops the timer (resets it to 0).
        public static void Stop(ref float t, float T = 0f) {
            t = 0;
        }

        // Ticks the timer down by the given interval.
        public static bool TickDown(ref float t, float dt) {
            bool wasnotzero = t > 0f;
            bool isnowzero = false;
            t -= dt;
            if (t <= 0f) {
                t = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Ticks the timer down by the given interval if the predicate is fulfilled.
        public static bool TickDownIf(ref float t, float dt, bool p) {
            bool wasnotzero = t > 0f;
            bool isnowzero = false;
            if (p) {  t -= dt; }
            if (t <= 0f) {
                t = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Ticks the timer up to a specified maximum by the given interval.
        public static bool TickUp(ref float t, float T, float dt) {
            bool wasnotmax = t < T;
            bool isnowmax = false;
            t += dt;
            if (t >= T) {
                t = T;
                isnowmax = true;
            }
            return wasnotmax && isnowmax;
        }

        // Ticks the timer up by the given interval  a specified maximum if the predicate is fulfilled.
        public static bool TickUpIf(ref float t, float T, float dt, bool p) {
            bool wasnotmax = t < T;
            bool isnowmax = false;
            if (p) { t += dt; }
            if (t >= T) {
                t = T;
                isnowmax = true;
            }
            return wasnotmax && isnowmax;
        }

         public static bool Cycle(ref float ticks, float buffer, float dt) {
            ticks += dt;
            if (ticks > 2f * buffer) {
                ticks -= 2f * buffer;
            }
            return ticks > buffer;
        }

    }

}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    public class Timer {

        public static bool UpdateTicks(ref float ticks, bool condition, float buffer, float dt) {
            bool wasnotzero = ticks != 0f;
            if (condition) {
                ticks += dt;
                if (ticks >= buffer) {
                    ticks = buffer;
                }
            }
            else {
                ticks -= dt;
                if (ticks < 0f) {
                    ticks = 0f;
                }
            }
            bool isnowzero = ticks == 0f;
            return wasnotzero && isnowzero;
        }

        public static bool CycleTicks(ref float ticks, float buffer, float dt) {
            ticks += dt;
            if (ticks > 2f * buffer) {
                ticks -= 2f * buffer;
            }
            return ticks > buffer;
        }

        public static bool CountdownTicks(ref float ticks, bool condition, float buffer, float dt) {
            bool wasnotzero = ticks != 0f;
            if (condition) {
                ticks = buffer;
            }
            else {
                ticks -= dt;
                if (ticks < 0f) {
                    ticks = 0f;
                }
            }
            bool isnowzero = ticks == 0f;
            return wasnotzero && isnowzero;
        }

        public static void Start(ref float ticks, float buffer) {
            ticks = buffer; 
        }

        public static void StartIf(ref float ticks, float buffer, bool predicate) {
            if (predicate) { 
                ticks = buffer; 
            }
        }

        public static bool TickDown(ref float ticks, float dt) {
            bool wasnotzero = ticks > 0f;
            ticks -= dt;
            bool isnowzero = ticks <= 0f;
            if (isnowzero) {
                ticks = 0f;
            }
            return wasnotzero && isnowzero;
        }

        public static bool TickDownIf(ref float ticks, float dt, bool predicate) {
            bool wasnotzero = ticks > 0f;
            if (predicate) { 
                ticks -= dt;
            }
            if (ticks < 0f) {
                ticks = 0f;
            }
            bool isnowzero = ticks <= 0f;
            return wasnotzero && isnowzero;
        }

        public static bool TickUp(ref float ticks, float max, float dt) {
            bool wasnotmax = ticks < max;
            ticks += dt;
            bool isnowmax = ticks >= max;
            if (ticks >= max) {
                ticks = max;
            }
            return wasnotmax && isnowmax;
        }

        public static bool TickUpIf(ref float ticks, float max, float dt, bool predicate) {
            bool wasnotmax = ticks < max;
            if (predicate) {
                ticks += dt;
            }
            if (ticks > max) {
                ticks = max;
            }
            bool isnowmax = ticks >= max;
            return wasnotmax && isnowmax;
        }

        public static void Reset(ref float ticks, float value = 0f) {
            ticks = value;
        }

    }

}



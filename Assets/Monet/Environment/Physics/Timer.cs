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

    }

}



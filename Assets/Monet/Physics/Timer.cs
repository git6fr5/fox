using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    public class Timer {

        public static void UpdateTicks(ref float ticks, bool condition, float buffer, float dt) {
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
        }

        public static void CountdownTicks(ref float ticks, bool condition, float buffer, float dt) {
            if (condition) {
                ticks = buffer;
            }
            else {
                ticks -= dt;
                if (ticks < 0f) {
                    ticks = 0f;
                }
            }
        }

    }

}



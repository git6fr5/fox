/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.LevelLoader;
using Platformer.Obstacles;

namespace Platformer.LevelLoader {

    /// <summary>
    /// An entity object readable by the level loader.
    /// That specifically inteprets control data in order to get
    /// a position within a cycle.
    /// <summary>
    public class TimedEntity : Entity {

        // Take the control data and turn it into a period offset.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            int offset = TimedEntity.GetOffset(index, controlData);
            TimedSpike timedSpike = GetComponent<TimedSpike>();
            if (timedSpike != null) {
                timedSpike.Init(offset);
            }
            Spitter spitter = GetComponent<Spitter>();
            if (spitter != null) {
                spitter.Init(offset);
            }
            Spikeball spikeball = GetComponent<Spikeball>();
            if (spikeball != null) {
                spikeball.Init(offset);
            }

        }

        // The logic of turning the ldtk data into a period offset.
        public static int GetOffset(int index, List<LDtkTileData> controlData) {
            return controlData[index].VectorID.x;
        }
    }
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class TimedEntity : Entity {

        public override void OnControl(int index, List<LDtkTileData> controlData) {
            
            int offset = TimedEntity.GetOffset(index, controlData);
            TimedSpike timedSpike = GetComponent<TimedSpike>();
            if (timedSpike != null) {
                timedSpike.Init(offset);
            }

        }

        public static int GetOffset(int index, List<LDtkTileData> controlData) {
            return controlData[index].VectorID.x;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LDtkTileData = LevelLoader.LDtkTileData;

public class ElevatorPlatform : MovingPlatform {

    // Sets the target for this platform.
    protected override void Target() {
        bool foundPlayer = false;
        for (int i = 0; i < m_Container.Count; i++) {
            if (m_Container[i].GetComponent<Player>() != null) {
                foundPlayer = true;
                break;
            }
        }
        if (!foundPlayer) { return; }

        base.Target();
    }

    // Moves this platform.
    protected override void ProcessMovement(float deltaTime) {
        base.ProcessMovement(deltaTime);

    }
    
}

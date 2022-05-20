using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LDtkTileData = LevelLoader.LDtkTileData;

public class ElevatorPlatform : MovingPlatform {

    public void SwitchState() {
        // Sets the target for this platform.
        if (m_Path == null || m_Path.Length == 0) { return; }
        
        m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
        Debug.DrawLine(transform.position, m_Path[m_PathIndex], Color.white);
    }

    // Sets the target for this platform.
    protected override void Target() {
        // nothing.
    }

    // Moves this platform.
    protected override void ProcessMovement(float deltaTime) {
        base.ProcessMovement(deltaTime);

    }
    
}

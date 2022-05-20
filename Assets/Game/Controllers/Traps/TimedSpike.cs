/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class TimedSpike : Spike {

    private static float Interval = 2f;

    [SerializeField, ReadOnly] private float m_TickOffset;
    [SerializeField, ReadOnly] private float m_Ticks;

    void FixedUpdate() {
        float deltaTime = Time.deltaTime;
        m_Ticks += deltaTime;

        if (m_Ticks > Interval) {
            m_Ticks -= Interval;
            Flip();
        }
    }

    public void Init(int offset) {
        m_TickOffset = (float)offset * Interval / 2f;
    }

    public override void Init() {
        m_Ticks = m_TickOffset;
        base.Init();
        m_SpriteRenderer.enabled = true;
        m_Hitbox.enabled = true;
    }

}
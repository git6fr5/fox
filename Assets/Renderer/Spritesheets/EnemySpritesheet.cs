/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class EnemySpritesheet : Spritesheet {

    [HideInInspector] private Enemy m_Enemy;

    [SerializeField] private int m_AggroIdleFrames;
    [HideInInspector] private Sprite[] m_AggroAnimation;
    


    public override void Init() {
        m_Enemy = GetComponent<Enemy>();
        base.Init();
    }

    public override int Organize() {
        int startIndex = base.Organize();
        startIndex = SliceSheet(startIndex, m_AggroIdleFrames, ref m_AggroAnimation);
        return startIndex;
    }

    public override Sprite[] GetAnimation() {
        m_FrameRate = GameRules.FrameRate;
        if (m_Enemy.Aggro) {
            return m_AggroAnimation;
        }
        return base.GetAnimation();
    }

    protected override void Rotate() {
        base.Rotate();
    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.VFX;

///<summary>
///
///<summary>
public class PlayerSpritesheet : Spritesheet {

    [HideInInspector] private Player m_Player;

    [Space(2), Header("Double Jump")]
    [SerializeField] private int m_DoubleJumpFrames;
    [HideInInspector] private Sprite[] m_DoubleJumpAnimation;

    [Space(2), Header("Wall Climbing")]
    [SerializeField] private int m_WallClimbIdleFrames;
    [SerializeField] private int m_WallClimbMovingFrames;
    [SerializeField] private int m_WallClimbRisingFrames;
    [SerializeField] private int m_WallClimbFallingFrames;

    // Animations
    [HideInInspector] private Sprite[] m_WallClimbIdleAnimation;
    [HideInInspector] private Sprite[] m_WallClimbMovingAnimation;
    [HideInInspector] private Sprite[] m_WallJumpRisingAnimation;
    [HideInInspector] private Sprite[] m_WallJumpFallingAnimation;
    

    [Space(2), Header("Dashing")]
    [SerializeField] private int m_DashFrames;
    [HideInInspector] private Sprite[] m_DashAnimation;
    [SerializeField] private VisualEffect m_DashEffect;

    public override void Init() {
        m_Player = GetComponent<Player>();
        base.Init();
    }

    public override int Organize() {
        int startIndex = base.Organize();
        startIndex = SliceSheet(startIndex, m_DoubleJumpFrames, ref m_DoubleJumpAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbIdleFrames, ref m_WallClimbIdleAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbMovingFrames, ref m_WallClimbMovingAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbRisingFrames, ref m_WallJumpRisingAnimation);
        startIndex = SliceSheet(startIndex, m_WallClimbFallingFrames, ref m_WallJumpFallingAnimation);
        startIndex = SliceSheet(startIndex, m_DashFrames, ref m_DashAnimation);
        return startIndex;
    }

    public override Sprite[] GetAnimation() {

        m_FrameRate = GameRules.FrameRate;
        if (m_Player.Dashing) {
            if (m_CurrentAnimation != m_DashAnimation) {
                m_DashEffect.Play();
            }
            return m_DashAnimation;
        }

        if (m_Player.DoubleJumping) {
            if (m_CurrentAnimation != m_DoubleJumpAnimation) {
                m_DashEffect.Play();
            }
            return m_DoubleJumpAnimation;
        }

        if (m_Player.WallClimbing) {
            if (m_Player.ClimbInput != 0f) {
                return m_WallClimbMovingAnimation;
            }
            else {
                return m_WallClimbIdleAnimation;
            }
        }
        if (m_Player.WallJumping) {
            switch (m_Player.AirborneFlag) {
                case Controller.Airborne.Rising:
                    return m_WallJumpRisingAnimation;
                default:
                    return m_WallJumpFallingAnimation;
            }
        }

        return base.GetAnimation();
    }

    protected override void Rotate() {
        if (m_Player.Dashing) {
            float angle = Vector2.SignedAngle(Vector2.right, m_Player.Body.velocity);
            print(angle);
            transform.eulerAngles = angle * Vector3.forward;
            return;
        }
        base.Rotate();
    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using LDtkTileData = LevelLoader.LDtkTileData;

///<summary>
///
///<summary>
public class Enemy : Controller {

    /* --- Variables --- */
    #region Variables
    
    [HideInInspector] protected Vector3[] m_Path = null;
    [SerializeField, ReadOnly] protected int m_PathIndex;

    [SerializeField] private float m_PathDuration;
    [SerializeField] private float m_Ticks;
    [SerializeField, ReadOnly] private float m_Direction = 1f;

    [SerializeField] private VisionCone m_VisionCone;

    public bool Aggro = false;
    [SerializeField] private float m_AggroBufferInterval = 1f;
    [SerializeField, ReadOnly] private float m_AggroTicks;
    
    #endregion

    public void InitPath(int index, List<LDtkTileData> controlData) {

        Vector2 _direction = Quaternion.Euler(0f, 0f, 90f * controlData[index].vectorID.x) * Vector2.right;
        Vector2Int direction = new Vector2Int((int)Mathf.Round(_direction.x), (int)Mathf.Round(_direction.y));
        print(direction);
        direction.y *= -1;

        List<LDtkTileData> nodes = new List<LDtkTileData>();
         for (int i = 0; i < controlData.Count; i++) {
            if (controlData[i].vectorID == new Vector2Int(0, 1)) {
                nodes.Add(controlData[i]);
            }
        }

        int distance = 0;
        bool continueSearch = true;
        while (continueSearch && distance < 50) {
            distance += 1;
            
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].gridPosition == controlData[index].gridPosition + distance * direction) {
                    continueSearch = false;
                    break;
                }
            }

        }

        List<Vector3> l_Path = new List<Vector3>();
        l_Path.Add(transform.position);
        l_Path.Add(transform.position + (distance) * (Vector3)_direction);
        m_Path = l_Path.ToArray();

    }

    /* --- Overrides --- */
    #region Overrides
    
    protected override void GetInput() {

        bool wasAggro = Aggro;
        Aggro = m_VisionCone.Active;

        if (Aggro && !wasAggro) {
            JustSawPlayer();
            return;
        }

        if (!Aggro) {
            IdleBehaviour();
        } 
        else {
            AggroBehaviour();
        }
        
    }

    private void IdleBehaviour() {
        // if (m_Ticks >= m_PathDuration) {
        //     m_Direction *= -1f;
        //     m_Ticks -= m_PathDuration;
        // }

        Target();
        Vector3 displacement = m_Path[m_PathIndex] - transform.position;
        if (displacement.sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            m_MoveInput = 0f;
            return;
        }
        else {
            m_MoveInput = Mathf.Sign(displacement.x);
        }

        m_JumpInput = false;
        m_FloatInput = false;
        m_AttackInput = false;

    }

    private void AggroBehaviour() {
        float playerDirection = Mathf.Sign((m_VisionCone.player.transform.position - transform.position).x);
        float direction = DirectionFlag == Direction.Right ? 1f : -1f;
        if (direction != playerDirection) {
            m_MoveInput = playerDirection;
        }
        else {
            m_MoveInput = 0f;
        }

        m_FloatInput = true;

        m_JumpInput = false;
        // if (m_Projectile.WillThisBeReadyToFireIn(0.2f)) {
        //     m_JumpInput = true;
        // }
        m_AttackInput = m_AggroTicks >= m_AggroBufferInterval ? true : (m_AggroTicks <= 0f ? false : m_AttackInput);

        Vector2 playerPosition = m_VisionCone.player.transform.position;
        Vector2 playerDisplacement = (Vector2)playerPosition - (Vector2)transform.position;
        m_AttackDirection = playerDisplacement.normalized; // + Vector2.up * Mathf.Abs(playerDisplacement.x) / (0.25f * m_Projectile.Speed * m_Projectile.Speed);
    }

    private void JustSawPlayer() {
        m_MoveInput = 0f;
        m_JumpInput = true;
        m_FloatInput = true;
        m_AttackInput = false;
    }

    // Sets the target for this platform.
    protected virtual void Target() {
        if (m_Path == null || m_Path.Length == 0) { return; }
        
        if ((m_Path[m_PathIndex] - transform.position).sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
        }
        Debug.DrawLine(transform.position, m_Path[m_PathIndex], Color.white);

    }

    protected override void ProcessThink(float deltaTime) {
        m_Ticks += deltaTime;

        if (m_VisionCone.Active) {
            m_AggroTicks += deltaTime;
        }
        else if (!m_VisionCone.Active) {
            m_AggroTicks -= deltaTime;
        }
        m_AggroTicks = m_AggroTicks >= m_AggroBufferInterval ? m_AggroBufferInterval : (m_AggroTicks <= 0f ? 0f : m_AggroTicks);

        base.ProcessThink(deltaTime);
    }

    #endregion

}
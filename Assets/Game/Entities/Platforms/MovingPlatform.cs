using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LDtkTileData = LevelLoader.LDtkTileData;

public class MovingPlatform : Platform {

    [HideInInspector] protected Vector3[] m_Path = null;
    [SerializeField, ReadOnly] protected int m_PathIndex;
    [SerializeField, ReadOnly] protected List<Controller> m_Container = new List<Controller>();
    [SerializeField] protected float m_Speed = 3f;

    public override void Init(int index, List<LDtkTileData> controlData) {
        Init();

        Vector2 _direction = Quaternion.Euler(0f, 0f, 90f * controlData[index].vectorID.x) * Vector2.right;
        Vector2Int direction = new Vector2Int((int)Mathf.Round(_direction.x), (int)Mathf.Round(_direction.y));
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
        if (direction.x != 0f) {
            l_Path.Add(transform.position + (distance - m_Length) * (Vector3)_direction);
        }
        else {
            l_Path.Add(transform.position + (distance) * (Vector3)_direction);
        }
        m_Path = l_Path.ToArray();

    }

    /* --- Unity --- */
    #region Unity

    void Update() {
        Target();
    }

    void FixedUpdate() {
        float deltaTime = Time.deltaTime;
        ProcessMovement(deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Controller controller = collision.gameObject.GetComponent<Controller>();
        if (controller != null && !m_Container.Contains(controller)) {
            m_Container.Add(controller);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        Controller controller = collision.gameObject.GetComponent<Controller>();
        if (controller != null && m_Container.Contains(controller)) {
            m_Container.Remove(controller);
        }
    }

    #endregion
    
    /* --- Movement --- */
    #region Movement
    
    // Sets the target for this platform.
    protected virtual void Target() {
        if (m_Path == null || m_Path.Length == 0) { return; }
        
        if ((m_Path[m_PathIndex] - transform.position).sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
        }
        Debug.DrawLine(transform.position, m_Path[m_PathIndex], Color.white);

    }

    // Moves this platform.
    protected virtual void ProcessMovement(float deltaTime) {
        if (m_Path == null || m_Path.Length == 0) { return; }

        Vector3 displacement = m_Path[m_PathIndex] - transform.position;
        if (displacement.sqrMagnitude < GameRules.MovementPrecision * GameRules.MovementPrecision) {
            transform.position = m_Path[m_PathIndex];
            return;
        }

        Vector3 velocity = displacement.normalized * m_Speed;
        transform.position += velocity * deltaTime;
        for (int i = 0; i < m_Container.Count; i++) {
            m_Container[i].transform.position += velocity * deltaTime;
        }

    }
    
    #endregion

    
}

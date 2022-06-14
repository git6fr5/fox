/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using LDtkTileData = LevelLoader.LDtkTileData;

///<summary>
///
///<summary>
public class Enemy : Input {

    /* --- Variables --- */
    #region Variables
    
    [HideInInspector] protected Vector3[] m_Path = null;
    [SerializeField, ReadOnly] protected int m_PathIndex;

    [SerializeField] private float m_PathDuration;
    [SerializeField] private float m_Ticks;

    #endregion

    public void InitPath(int index, List<LDtkTileData> controlData) {

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
        l_Path.Add(transform.position + (distance) * (Vector3)_direction);
        m_Path = l_Path.ToArray();

    }

    public static int m_Precision = 3;

    [SerializeField] private float m_Angle = 30f;
    [SerializeField] private float m_PassiveRange = 5f;
    [SerializeField] private float m_ActiveRange = 12.5f;
    [SerializeField] protected float m_AttackRange = 7.5f;

    [HideInInspector] private List<Vector3> m_LightRays;

    [SerializeField] private bool m_Active;
    public bool Active => m_Active;

    [HideInInspector] protected Controller m_Controller;
    [HideInInspector] private Player m_Player;
    public Player player => m_Player;

    [SerializeField] private LayerMask m_Mask;

    void Start() {
        m_Controller = GetComponent<Controller>();
    }
    
    public override void Update() {
        GetMove();
        GetAttack();
        GetJump();
        GetDash();
    }

    public virtual void GetMove() {}

    public virtual void  GetAttack() {}

    public virtual void  GetJump() {}

    public virtual void  GetDash() {}

        
    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Search(deltaTime);
        Timers(deltaTime);
    }

    public void Search(float deltaTime) {
        m_LightRays = new List<Vector3>();
        if (m_Active) {
            m_Active = ActiveSearch(deltaTime);
            return;
        }
        m_Active = PassiveSearch(deltaTime);
    }

    [System.Serializable]
    public class Timer {
        public bool active = false;
        public bool Active => active;
        public float ticks = 0f;
        public float duration = 1f;

        public Timer(float duration) {
            this.duration = duration;
        }

        public bool CheckFinished(bool reset = true) {
            if (!active) { return true; }

            if (ticks >= duration) {
                if (reset) { Stop(); }
                return true;
            }
            return false;
        }

        public void Start() {
            print("starting");
            active = true;
            ticks = 0f;
        }

        public void Stop() {
            active = false;
            ticks = 0f;
        }

        public void Continue() {
            active = true;
        }

        public void Pause() {
            active = false;
        }

        public void Update(float deltaTime) {
            if (active) {
                ticks += deltaTime;
            }
        }

    }


    public virtual void Timers(float deltaTime) {
        
    }

    public float forwardDist = 0.5f;
    public float precision = 3;

    private bool PassiveSearch(float deltaTime) {

        bool foundPlayer = false;
        for (int i = 0; i < precision; i++) {

            // Set up the ray.
            Vector3 start = transform.position + m_Controller.State.Direction * Vector3.right * forwardDist;
            Vector3 baseDirection = Vector3.right * m_Controller.State.Direction;
            Vector3 direction = Quaternion.Euler(0f, 0f, -m_Angle + i * (2f * m_Angle / precision)) * baseDirection;
            float distance = m_PassiveRange;

            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, m_Mask);
            if (hit.collider != null && hit.collider.gameObject != m_Controller.gameObject) {
                // print("Hitting Something " + hit.collider.name);
                distance = (transform.position - (Vector3)hit.point).magnitude;
                Player player = hit.collider.GetComponent<Player>();
                if (player != null) {
                    m_Player = player;
                    foundPlayer = true;
                }
            }

            m_LightRays.Add(distance * direction);
            Debug.DrawLine(start, start + distance * direction, Color.yellow, deltaTime);
            
        }

        if (foundPlayer) {
            return true;
        }

        bool aggroedEnemyNearby = false;
        bool playerWithinActiveRange = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_ActiveRange + 2f * GameRules.MovementPrecision, m_Mask);
        for (int i = 0; i < colliders.Length; i++) {

            Enemy enemy = colliders[i].GetComponent<Enemy>();
            if (enemy != null && enemy.Active) {
                aggroedEnemyNearby = true;
            }

            Player player = colliders[i].GetComponent<Player>();
            if (player != null) {
                m_Player = player;
                playerWithinActiveRange = true;
            }
        }

        if (playerWithinActiveRange && m_Player.GetComponent<Controller>().State.IsHot) {
            return ActiveSearch(deltaTime);
        }
        if (aggroedEnemyNearby && playerWithinActiveRange) {
            return ActiveSearch(deltaTime);
        }

        return false;
    }

    private bool ActiveSearch(float deltaTime) {

        bool foundPlayer = false;

        // Set up the ray.
        Vector3 start = transform.position;
        Vector3 direction = (m_Player.transform.position - start).normalized;
        start += direction * 2 * forwardDist;
        float distance = m_ActiveRange;

        // Cast the ray.
        RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, m_Mask);
        if (hit.collider != null && hit.collider.gameObject != gameObject) {
            print("Hitting Something " + hit.collider.name);
            distance = (transform.position - (Vector3)hit.point).magnitude;
            Player player = hit.collider.GetComponent<Player>();
            if (player != null) {
                m_Player = player;
                foundPlayer = true;
            }
        }

        Debug.DrawLine(start, start + distance * direction, Color.yellow, deltaTime);
        return foundPlayer;
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, m_ActiveRange);
    }

}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class VisionCone : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    public static int m_Precision = 3;

    [SerializeField] private float m_Angle = 30f;
    [SerializeField] private float m_PassiveRange = 3f;
    [SerializeField] private float m_ActiveRange = 7.5f;

    [HideInInspector] private List<Vector3> m_LightRays;

    [SerializeField] private bool m_Active;
    public bool Active => m_Active;

    [HideInInspector] private Controller m_Controller;
    [HideInInspector] private Player m_Player;
    public Player player => m_Player;

    [SerializeField] private LayerMask m_Mask;
    
    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        m_Controller = transform.parent.GetComponent<Controller>();
    }
    
    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Search(deltaTime);
    }
    
    #endregion

    public void Search(float deltaTime) {

        // print("Looking for player");

        m_LightRays = new List<Vector3>();
        if (m_Active) {
            m_Active = ActiveSearch(deltaTime);
            return;
        }
        m_Active = PassiveSearch(deltaTime);
    }

    private bool PassiveSearch(float deltaTime) {

        bool foundPlayer = false;
        for (int i = 0; i < m_Precision; i++) {

            // Set up the ray.
            Vector3 start = transform.position;
            Vector3 baseDirection = m_Controller.DirectionFlag == Controller.Direction.Right ? Vector3.right : Vector3.left;
            Vector3 direction = Quaternion.Euler(0f, 0f, -m_Angle + i * (2f * m_Angle / m_Precision)) * baseDirection;
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

        return foundPlayer;
    }

    private bool ActiveSearch(float deltaTime) {

        bool foundPlayer = false;

        // Set up the ray.
        Vector3 start = transform.position;
        Vector3 direction = (m_Player.transform.position - start).normalized;
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

}
/* --- Unity --- */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameRules : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Instance.
    public static GameRules Instance;

    // Player.
    [SerializeField] public Player mainPlayer;
    public static Player MainPlayer => Instance.mainPlayer;
    
    // Coin.
    [SerializeField] public  Coin m_Coin;
    public static Coin MainCoin => Instance.m_Coin;

    // Movement.
    [SerializeField] private float movementPrecision = 0.05f;
    public static float MovementPrecision => Instance.movementPrecision;
    [SerializeField] private float gravityScale = 1f;
    public static float GravityScale => Instance.gravityScale;
    
    // Ticks.
    public static float Ticks;
    [SerializeField] private float m_FrameRate = 1f;
    public static float FrameRate => Instance.m_FrameRate;

    // Rendering Layers.
    public static string BorderRenderingLayer = "Border";
    public static string BackgroundRenderingLayer = "Background";
    public static string ForegroundRenderingLayer = "Foreground";
    public static string UIRenderingLayer = "UI";

    // Layers
    [SerializeField] private LayerMask groundCollisionLayer;
    public static LayerMask GroundCollisionLayer => Instance.groundCollisionLayer;
    
    [SerializeField] private LayerMask wallCollisionLayer;
    public static LayerMask WallCollisionLayer => Instance.wallCollisionLayer;

    [SerializeField] private LayerMask waterCollisionLayer;
    public static LayerMask WaterCollisionLayer => Instance.waterCollisionLayer;
    
    [SerializeField] private LayerMask climbCollisionLayer;
    public static LayerMask ClimbCollisionLayer => Instance.climbCollisionLayer;
    
    [SerializeField] private LayerMask interactableCollisionLayer;
    public static LayerMask InteractableCollisionLayer => Instance.interactableCollisionLayer;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start(){
        Init();
    }

    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Ticks += deltaTime;
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        Instance = this;
    }

    #endregion

    /* --- Generics --- */
    #region Generics

    #endregion

}


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
    [SerializeField] private Player mainPlayer;
    public static Player MainPlayer => Instance.mainPlayer;

    // Movement.
    [SerializeField] private float movementPrecision = 0.05f;
    public static float MovementPrecision => Instance.movementPrecision;
    [SerializeField] private float gravityScale = 1f;
    public static float GravityScale => Instance.gravityScale;
    
    // Ticks.
    [SerializeField] private float timeScale = 1f;
    public static float Ticks;
    [SerializeField] private float m_FrameRate = 1f;
    public static float FrameRate => Instance.m_FrameRate;

    // Rendering Layers.
    public static string BackgroundRenderingLayer = "Background";

    // Layers
    [SerializeField] private LayerMask groundCollisionLayer;
    public static LayerMask GroundCollisionLayer => Instance.groundCollisionLayer;
    

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start(){
        Init();
    }

    private void Update() {
        Time.timeScale = timeScale;
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


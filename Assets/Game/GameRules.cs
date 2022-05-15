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

    // Tags.
    public static string PlayerTag = "Player";

    // Instance.
    public static GameRules Instance;

    // Movement.
    [SerializeField] private float movementPrecision = 0.05f;
    public static float MovementPrecision => Instance.movementPrecision;
    [SerializeField] private float gravityScale = 1f;
    public static float GravityScale => Instance.gravityScale;
    
    // Ticks.
    [SerializeField] private float timeScale = 1f;
    public static float Ticks;

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


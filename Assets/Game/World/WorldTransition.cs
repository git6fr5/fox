/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;
using LDtkLayer = LDtkUnity.LayerInstance;
using LDtkTile = LDtkUnity.TileInstance;

using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class WorldTransition : MonoBehaviour
{

    public string sceneName;
    public string levelName;
    public static string LevelName = "";
    // [SerializeField] protected LDtkComponentProject m_LDtkData;

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.GetComponent<Player>()) {
            Activate();
        }
    }

    public void Activate() {
        LevelName = levelName;
        SceneManager.LoadScene(sceneName);
    }

}

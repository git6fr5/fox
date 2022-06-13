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

using Unity.SceneManagment;

public class WorldTransition : MonoBehaviour
{

    public Scene scene;
    // [SerializeField] protected LDtkComponentProject m_LDtkData;
    public WorldLoader worldLoader;

    void OnTriggerEnter2D(Collider2D collider) {

        if (collider.GetComponent<Player>()) {
            Activate();
        }

    }

    public void Activate() {

        SceneManager.LoadScene(sceneName);

    }

}

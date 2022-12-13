/* --- Libraries --- */
// Systems.
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using UnityEngine.U2D;

/* --- Definitions --- */
using Game = Platformer.GameManager;
using PixelPerfectCamera = UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

namespace Platformer {

    /// <summary>
    /// Controls the camera.
    /// </summary>
    [DefaultExecutionOrder(-900)]
    [RequireComponent(typeof(Camera)), RequireComponent(typeof(PixelPerfectCamera))]
    public class CameraController : MonoBehaviour {

        void Update() {
            transform.position = new Vector3(Game.MainPlayer.transform.position.x, Game.MainPlayer.transform.position.y, transform.position.z);
        }


    }

}

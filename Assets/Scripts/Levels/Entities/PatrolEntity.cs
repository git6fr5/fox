/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.LevelLoader;

namespace Platformer.LevelLoader {

    /// <summary>
    /// An entity object readable by the level loader.
    /// That specifically inteprets control data in order to derive
    /// a path.
    /// <summary>
    public class PatrolEntity : Entity {

        // Take the control data and turn it into a path.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            Vector3[] path = PatrolEntity.GetPath(transform.position, index, controlData);
            // Enemy enemy = GetComponent<Enemy>();
            // if (enemy != null) {
            //     enemy.Init(path);
            // }
        }

        // The logic of turning the ldtk data into a path.
        public static Vector3[] GetPath(Vector3 position, int index, List<LDtkTileData> controlData, int length = 0) {

            // Cache the direction of the path.
            Vector2 temp = Quaternion.Euler(0f, 0f, -90f * controlData[index].VectorID.x) * Vector2.up;
            Vector2Int direction = new Vector2Int((int)Mathf.Round(temp.x), (int)Mathf.Round(temp.y));
            direction.y *= -1;

            // Cache all the stop nodes in the scene.
            List<LDtkTileData> stopNodes = controlData.FindAll(data => data.VectorID == LDtkTileData.ControlStopID);

            // Itterate in the direction of the path until reaching a stop node.
            int distance = 0;
            bool continueSearch = true;
            while (continueSearch && distance < 50) {
                distance += 1;
                LDtkTileData stopNode = stopNodes.Find(node => node.GridPosition == controlData[index].GridPosition + distance * direction);
                if (stopNode != null) {
                    continueSearch = false;
                    break;
                }
            }

            // Convert the start and end nodes into world positions.
            List<Vector3> path = new List<Vector3>();
            path.Add(position);
            if (direction.x != 0f) {
                path.Add(position + (distance - length) * (Vector3)temp);
            }
            else {
                path.Add(position + (distance) * (Vector3)temp);
            }
            return path.ToArray();

        }

    }
}
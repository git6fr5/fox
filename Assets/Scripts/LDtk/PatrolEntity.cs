/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class PatrolEntity : Entity {

        public override void OnControl(int index, List<LDtkTileData> controlData) {
            Vector3[] path = PatrolEntity.GetPath(transform.position, index, controlData);
            
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null) {
                enemy.Init(path);
            }

        }

        public static Vector3[] GetPath(Vector3 position, int index, List<LDtkTileData> controlData, int length = 0) {

            Vector2 temp = Quaternion.Euler(0f, 0f, -90f * controlData[index].VectorID.x) * Vector2.up;
            Vector2Int direction = new Vector2Int((int)Mathf.Round(temp.x), (int)Mathf.Round(temp.y));
            direction.y *= -1;

            List<LDtkTileData> stopNodes = controlData.FindAll(data => data.VectorID == LDtkTileData.ControlStopID);

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
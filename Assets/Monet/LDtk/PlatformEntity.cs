/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class PlatformEntity : Entity {

        public override void OnControl(int index, List<LDtkTileData> controlData) {
            
            int length = PlatformEntity.GetLength(transform.position);
            Vector3[] path = PatrolEntity.GetPath(transform.position, index, controlData, length);
            
            Platform platform = GetComponent<Platform>();
            if (platform != null) {
                platform.Init(length, path);
            }

        }

        public static int GetLength(Vector3 position) {
            List<PlatformEntity> garbage = new List<PlatformEntity>();

            int length = 0;
            bool continueSearch = true;
            while (continueSearch && length < 50) {
                length += 1;

                continueSearch = false;
                Vector3 offset = ((length - 1f) + 0.5f + Game.Physics.MovementPrecision) * Vector3.right;
                RaycastHit2D hit = Physics2D.Raycast(position + offset, Vector2.right, 0.25f, Game.Physics.CollisionLayers.Platform);
                if (hit.collider != null && hit.collider.GetComponent<PlatformEntity>() != null) {
                    continueSearch = true;
                    garbage.Add(hit.collider.GetComponent<PlatformEntity>());
                }

            }

            for (int i = 0; i < garbage.Count; i++) {
                Destroy(garbage[i].gameObject);
            }

            return length;
        }
    }
}
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
    /// That specifically inteprets control data in order to set up
    /// a platform.
    /// <summary>
    public class PlatformEntity : Entity {

        // Take the control data and turn it into a length and path.
        public override void OnControl(int index, List<LDtkTileData> controlData) {
            int length = PlatformEntity.GetLength(transform.position);
            Vector3[] path = PatrolEntity.GetPath(transform.position, index, controlData, length);
            // Platform platform = GetComponent<Platform>();
            // if (platform != null) {
            //     platform.Init(length, path);
            // }
        }

        // The logic of turning the ldtk data into a length.
        public static int GetLength(Vector3 position) {
            // To cache the entities we want to delete after.
            List<PlatformEntity> garbage = new List<PlatformEntity>();

            // Itterate right until we are no longer touching
            // a platform entity.
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

            // Delete the garabage collected.
            for (int i = 0; i < garbage.Count; i++) {
                Destroy(garbage[i].gameObject);
            }
            return length;
        }
    }
}
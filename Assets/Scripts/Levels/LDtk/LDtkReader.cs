/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;
// Platformer.
using Platformer.LevelLoader;


namespace Platformer.LevelLoader {

    ///<summary>
    ///
    ///<summary>
    public class LDtkReader : MonoBehaviour {

        // Grid Size
        public static int GridSize = 16;

        public static List<LDtkTileData> GetLayerData(LDtkUnity.Level ldtkLevel, string layerName) {
            List<LDtkTileData> layerData = new List<LDtkTileData>();

            LDtkUnity.LayerInstance layer = GetLayerInstance(ldtkLevel, layerName);
            if (layer != null) { 
                for (int index = 0; index < layer.GridTiles.Length; index++) {
                    LDtkUnity.TileInstance tile = layer.GridTiles[index];
                    LDtkTileData tileData = new LDtkTileData(GetVectorID(tile), GetGridPosition(tile), index);
                    layerData.Add(tileData);
                }
            }
            return layerData;
        }

        public static LDtkUnity.LayerInstance GetLayerInstance(LDtkUnity.Level ldtkLevel, string layerName) {
            for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
                LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
                if (layer.IsTilesLayer && layer.Identifier == layerName) {
                    return layer;
                }
            }
            return null;
        }

        private static Vector2Int GetVectorID(LDtkUnity.TileInstance tile) {
            return new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / GridSize;
        }

        private static Vector2Int GetGridPosition(LDtkUnity.TileInstance tile) {
            return tile.UnityPx / GridSize;
        }

        protected Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
            LDtkTileData tileData = data.Find(tileData => tileData != null && tileData.GridPosition == gridPosition);
            return (Vector2Int?)tileData?.VectorID;
        }

    } 

}
    
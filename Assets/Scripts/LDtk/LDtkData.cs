/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
using Monet;

namespace Monet {

    ///<summary>
    /// A useful data structure for reading and matching LDtk data to game data.
    ///<summary>
    public class LDtkTileData {

        public static Vector2Int ControlStopID = new Vector2Int(1, 0);
        
        private Vector2Int m_VectorID;
        public Vector2Int VectorID => m_VectorID;

        private Vector2Int m_GridPosition;
        public Vector2Int GridPosition => m_GridPosition;

        private int m_Index;
        public int Index => m_Index;

        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            m_VectorID = vectorID;
            m_GridPosition = gridPosition;
            m_Index = index;
        }

    }

    public class LDtkLayer {

        // Layer Names
        public static string Control = "Controls";
        public static string Entity = "Entities";
        public static string Water = "Water";
        public static string Ground = "Ground";
        
    }

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
    
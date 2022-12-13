/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
// Platformer.
using Platformer.LevelLoader;
using Platformer.CustomTiles;

namespace Platformer.LevelLoader {

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Grid))]
    public class TilemapManager : MonoBehaviour {

        /* --- Variables --- */
        #region Variables

        // The ground map.
        private Tilemap m_GroundMap;
        public Tilemap GroundMap => m_GroundMap;

        // The ground map mask.
        private Tilemap m_GroundMaskMap;
        public Tilemap GroundMaskMap => m_GroundMaskMap;

        // The water map.
        private Tilemap m_WaterMap;
        public Tilemap WaterMap => m_WaterMap;

        // The tiles for the ground map.
        [SerializeField] 
        private GroundTile m_Ground;
        public GroundTile Ground => m_Ground; 

        // The tiles for the ground mask.
        [SerializeField] 
        private GroundTile m_GroundMask;
        public GroundTile GroundMask => m_GroundMask; 

        // The tiles for the ground water.
        [SerializeField] 
        private WaterTile m_Water;
        public WaterTile Water => m_Water;
        

        #endregion

        public void OnGameLoad() {
            InitializeGroundLayer();
            InitializeGroundMaskLayer();
            InitializeWaterLayer();
        }

        public void InitializeGroundLayer() {
            m_GroundMap = new GameObject("Ground", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
            // m_GroundMap.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Foreground;
            // m_GroundMap.color = Screen.ForegroundColorShift;

            m_GroundMap.gameObject.AddComponent<Rigidbody2D>();
            m_GroundMap.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            m_GroundMap.gameObject.AddComponent<CompositeCollider2D>();
            m_GroundMap.gameObject.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
            m_GroundMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
            // GroundMap.gameObject.AddComponent<ShadowCaster2DTileMap>();

            m_GroundMap.transform.SetParent(transform);
            m_GroundMap.transform.localPosition = Vector3.zero;
            m_GroundMap.gameObject.layer = LayerMask.NameToLayer("Ground");

        }

        public void InitializeGroundMaskLayer() {
            m_GroundMaskMap = new GameObject("Ground Mask", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            // m_GroundMapMask.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Midground;
            m_GroundMaskMap.GetComponent<TilemapRenderer>().sortingOrder = 10000;
            // GroundMapMask.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            m_GroundMaskMap.transform.SetParent(transform);
            m_GroundMaskMap.transform.localPosition = Vector3.zero;
        }

        public void InitializeWaterLayer() {
            m_WaterMap = new GameObject("Water", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D)).GetComponent<Tilemap>();
            // m_WaterMap.GetComponent<TilemapRenderer>().sortingLayerName = Screen.RenderingLayers.Foreground;
            m_WaterMap.GetComponent<TilemapCollider2D>().isTrigger = true;
            m_WaterMap.transform.SetParent(transform);
            m_WaterMap.transform.localPosition = Vector3.zero;
            m_WaterMap.gameObject.layer = LayerMask.NameToLayer("Water");
        }

    }
    
}
/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LDtkUnity;

using LDtkTileData = LevelLoader.LDtkTileData;

///<summary>
///
///<summary>
[DefaultExecutionOrder(1000)]
public class Minimap : MonoBehaviour {

    // public AnimatedTile playerTile;

    // public TileBase snailShell;
    // public TileBase hangingShell;
    // public TileBase coin;
    // public TileBase chest;
    // public TileBase grass;
    // public TileBase hangingGrass;

    // public TileBase fireSpirit;
    // public TileBase lightSpirit;

    public WorldLoader worldLoader;
    public Tilemap tilemap;
    public RuleTile groundTile;

    public TileBase respawnTile;
    public TileBase scrollTile;
    public TileBase shopKeeperTile; 
    public TileBase goldTile;

    public TileBase platformTile;
    public TileBase waterTile;

    public Tilemap playerMap;
    public TileBase playerTile;
    public Vector3Int playerPosition;

    void Start() { 
        LoadAllTiles(worldLoader.Levels);
    }

    void Update() {

        if (UnityEngine.Input.GetKeyDown(KeyCode.E)) {
            tilemap.gameObject.SetActive(!tilemap.gameObject.activeSelf);
            playerMap.gameObject.SetActive(tilemap.gameObject.activeSelf);
        }

        playerPosition = new Vector3Int((int)GameRules.MainPlayer.transform.position.x, (int)GameRules.MainPlayer.transform.position.y, 0);

        playerMap.ClearAllTiles();
        playerMap.SetTile(playerPosition, playerTile);

    }

    public void LoadAllTiles(Level[] levels) {
        for (int i = 0; i < levels.Length; i++) {

            LDtkUnity.Level ldtkLevel = levels[i].LDtkLevel; // GetLevel(level.LevelName);
            List<LDtkTileData> groundData = worldLoader.LoadLayer(ldtkLevel, WorldLoader.GroundLayer);
            List<LDtkTileData> waterData = worldLoader.LoadLayer(ldtkLevel, WorldLoader.WaterLayer);
            List<LDtkTileData> entityData = worldLoader.LoadLayer(ldtkLevel, WorldLoader.EntityLayer);

            AddTiles(groundData, groundTile, levels[i]);
            AddTiles(waterData, waterTile, levels[i]);
            AddTargets(entityData, levels[i]);

        }
        // m_Ground.RefreshAllTiles();
    }

    public void AddTiles(List<LDtkTileData> data, TileBase tile, Level level) {

        for (int i = 0; i < data.Count; i++) {
            
            Vector3Int position = level.GridToTilePosition(data[i].gridPosition);
            tilemap.SetTile(position, tile);

        }

    }

    public void AddTargets(List<LDtkTileData> entityData, Level level) {
        
        for (int i = 0; i < entityData.Count; i++) {
            Entity entityBase = Environment.GetEntityByVectorID(entityData[i].vectorID, worldLoader.Environment.Entities);
            if (entityBase != null) {

                Vector3Int position = level.GridToTilePosition(entityData[i].gridPosition);

                if (entityBase.gameObject.GetComponent<Platform>()) {
                    tilemap.SetTile(position, platformTile);
                }

                if (entityBase.gameObject.tag == "Respawn Anchor") {
                    tilemap.SetTile(position, respawnTile);
                }

                if (entityBase.gameObject.tag.Contains("scroll")) {
                    tilemap.SetTile(position, scrollTile);
                }

                if (entityBase.gameObject.tag == "Shopkeeper") {
                    tilemap.SetTile(position, shopKeeperTile);
                }

                if (entityBase.gameObject.tag == "Goldbox") {
                    tilemap.SetTile(position, goldTile);
                }
                
            }
            
        }

    }

}
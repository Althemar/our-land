﻿using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * CustomTile
 * Tile that inherate from TileBase
 */

public class CustomTile : TileBase
{
    public enum TerrainType
    {
        Grass, Mountain, Sand, Swamp, Water
    }

    public GameObject go;
    public bool canWalkThrough = true;
    public int walkCost = 1;

    public List<Sprite> centers;
    
    public List<Sprite> bordersNWEditor;
    public List<Sprite> bordersWEditor;
    public List<Sprite> bordersSWEditor;
    

    public Dictionary<TerrainType, List<Sprite>> bordersNW;
    public Dictionary<TerrainType, List<Sprite>> bordersW;
    public Dictionary<TerrainType, List<Sprite>> bordersSW;

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go) {
        if (go) {
            TileProperties properties = go.GetComponent<TileProperties>();
            if (properties) {
                HexagonalGrid grid = tilemap.GetComponent<HexagonalGrid>();
                properties.InitializeTile(location, this, grid, tilemap);
                grid.AddTile(properties);
            }
        }
        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        if (centers.Count > 0) {
            tileData.sprite = centers[Random.Range(0, centers.Count)];
        }
        else {
            tileData.sprite = null;
        }
        
        tileData.gameObject = go;
    }
    

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomTile")]
    public static void CreateCustomTile() {
        string path = EditorUtility.SaveFilePanelInProject("Save Custom Tile", "New Custom Tile", "Asset", "Save Custom Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTile>(), path);
    }
#endif
}

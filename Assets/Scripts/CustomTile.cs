using UnityEngine;
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
    public GameObject go;
    public bool canWalkThrough = true;
    public int walkCost = 1;

    public List<Sprite> centers;
    public List<Sprite> bordersNW;
    public List<Sprite> bordersW;
    public List<Sprite> bordersSW;

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
        tileData.sprite = centers[Random.Range(0, centers.Count)];
        tileData.gameObject = go;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {

        tilemap.RefreshTile(position);
        Debug.Log(tilemap.GetTile(position));
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

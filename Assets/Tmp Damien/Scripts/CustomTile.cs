using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomTile : TileBase
{
    public Sprite sprite;
    public bool canWalkThrough = true;
    public int walkCost = 1;

    Vector3Int position;

    public enum Direction { NW, W, SW, SE, E, NE }

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go) {
        position = location;
        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = sprite;
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

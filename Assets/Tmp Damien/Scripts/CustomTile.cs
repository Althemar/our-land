using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomTile : TileBase
{
    public Sprite sprite;
    public GameObject go;
    public bool canWalkThrough = true;
    public int walkCost = 1;

    ITilemap tilemap;

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go) {

        if (go) {
            TileProperties properties = go.GetComponent<TileProperties>();
            if (properties) {
                HexagonalGrid grid = tilemap.GetComponent<HexagonalGrid>();
                properties.InitializeTile(location, this, grid, tilemap);
                grid.AddTile(properties);
            }
        }

        this.tilemap = tilemap;
        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = sprite;
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

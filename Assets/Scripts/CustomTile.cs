using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
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
    public TerrainType terrainType;

    public string ambientRTPC = "";

    public List<Sprite> centers;
    
    [SerializeField]
    public BorderDictionary bordersNW;
    [SerializeField]
    public BorderDictionary bordersW;
    [SerializeField]
    public BorderDictionary bordersSW;
    /*
    public List<Sprite> addonsGrass;
    public List<Sprite> addonsFlowers;*/

    public AddonsDictionary addons;

    public bool humidityDependant = true;
    public bool riverSource = false;

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go) {
        if (HexagonalGrid.Instance && HexagonalGrid.Instance.GetTile(location))
            HexagonalGrid.Instance.GetTile(location).SetTile(this);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexagonalGrid : MonoBehaviour
{
    List<TileDatas> tiles;
    Tilemap tilemap;
    public HexCoordinates.HexCoordinatesType coordinatesType;
    

    public List<TileDatas> Tiles
    {
        get { return tiles; }
    }

    private void Awake() {
        tiles = new List<TileDatas>();
        coordinatesType = HexCoordinates.HexCoordinatesType.offset;
    }

    void Start() {

    }


    public void AddTile(TileDatas tile) {
        tiles.Add(tile);
    }

    public void ChangeCoordinateSystem(HexCoordinates.HexCoordinatesType type) {
        for (int i = 0; i < tiles.Count; i++) {
            tiles[i].Coordinates.ChangeCoordinatesType(type);
        }
        coordinatesType = type;
    }
}


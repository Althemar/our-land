using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDatas : MonoBehaviour
{
    Vector3Int position;
    HexCoordinates coordinates;
    CustomTile tile;

    public HexCoordinates Coordinates
    {
        get { return coordinates; }
    }

    public Vector3Int Position
    {
        get { return position; }
        set {
            position = value;
            coordinates = new HexCoordinates(position.x, position.y);
        }
    }

    public CustomTile Tile
    {
        get { return tile; }
        set { tile = value; }
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}

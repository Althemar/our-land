using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River {
    public Vector3Int source;
    public HexDirection directionFirst, directionSecond;
    public int distance;

    public TileProperties[] tiles;

    public River(Vector3Int source, HexDirection directionFirst, HexDirection directionSecond) {
        this.source = source;
        this.directionFirst = directionFirst;
        this.directionSecond = directionSecond;
    }

    public void ExtendRiver() {
        int distance = 10;

        while(distance > 0) {


            distance--;
        }
    }
}

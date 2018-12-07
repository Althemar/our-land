using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River {
    public HexCoordinates source;
    public HexDirection direction;
    public bool counterClockwise;
    public int distance;

    public TileProperties[] tiles;

    public River(HexCoordinates source, HexDirection direction, bool counterClockwise) {
        this.source = source;
        this.direction = direction;
        this.counterClockwise = counterClockwise;
        this.distance = 0;
    }

    public void ExtendRiver(HexagonalGrid grid) {
        TileProperties tile = grid.GetTile(source);
        if (!tile)
            return;
        
        TileProperties neigh = tile.GetNeighbor(counterClockwise ? direction.Previous().Previous() : direction.Next().Next());
        if (!neigh)
            return;

        neigh.SetRiver(direction, this);
        neigh.SetRiver(counterClockwise ? direction.Previous() : direction.Next(), this);

        //grid.SetColor(neigh.Position, Color.red);

        source = neigh.Coordinates;

        distance++;
    }
}

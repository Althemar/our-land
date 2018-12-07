using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River {
    public HexCoordinates source;
    public HexDirection direction;
    public bool counterClockwise;
    public int force;
    private int state;

    public River(HexCoordinates source, HexDirection direction, bool counterClockwise) {
        this.source = source;
        this.direction = direction;
        this.counterClockwise = counterClockwise;
        this.force = 5;
        this.state = 0;
    }

    public void ExtendRiver(HexagonalGrid grid) {
        TileProperties tile = grid.GetTile(source);
        if (!tile)
            return;

        TileProperties neigh = tile.GetNeighbor(counterClockwise ? direction.Previous().Previous() : direction.Next().Next());
        if (!neigh)
            return;

        if (state == 0) {
            force--;
            neigh.SetRiver(direction, this);
            state = 1;
        }
        else {
            force--;
            neigh.SetRiver(counterClockwise ? direction.Previous() : direction.Next(), this);
            source = neigh.Coordinates;
            state = 0;
        }

    }
}
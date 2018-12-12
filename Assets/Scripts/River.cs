using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River {
    public HexCoordinates source;
    public HexDirection direction;
    public bool counterClockwise;
    public int force;
    private int state;

    public bool doLake = true;

    public River(HexCoordinates source, HexDirection direction, bool counterClockwise, int force) {
        this.source = source;
        this.direction = direction;
        this.counterClockwise = counterClockwise;
        this.force = force;
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

    public void PutLac(HexagonalGrid grid) {
        if (!doLake)
            return;

        TileProperties tile = grid.GetTile(source);
        if (!tile)
            return;

        TileProperties neigh = tile.GetNeighbor(counterClockwise ? direction.Previous().Previous() : direction.Next().Next());
        if (!neigh)
            return;
        

        if (state == 0) {
            neigh.asLake = true;
            state = 1;
        }
        else {
            TileProperties lakePos = neigh.GetNeighbor(counterClockwise ? direction.Previous() : direction.Next());
            lakePos.asLake = true;
            state = 0;
        }
    }
}
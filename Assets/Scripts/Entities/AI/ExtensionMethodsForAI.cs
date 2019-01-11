using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethodsForAI
{
    public static TileProperties NearestBiomeWithoutEntities(this TileProperties tp, CustomTile.TerrainType biome, int maxDistance = -1) {
        List<TileProperties> visited = new List<TileProperties>();
        visited.Add(tp);

        List<TileProperties> fringes = new List<TileProperties>();
        List<TileProperties> nextfringes = new List<TileProperties>();

        fringes.Add(tp);
        if (tp.IsEmpty() && tp.Tile.terrainType == biome) {
            return tp;
        }
        int distanceDone = 1;
        while (fringes.Count > 0) {
            foreach (TileProperties previousTile in fringes) {
                TileProperties[] neighbors = previousTile.GetNeighbors();
                for (int j = 0; j < neighbors.Length; j++) {
                    TileProperties neighbor = neighbors[j];
                    if (neighbor && neighbor.Tile && !visited.Contains(neighbor) && neighbor.Tile.canWalkThrough) {
                        if (neighbor.IsEmpty() && neighbor.Tile.terrainType == biome) {
                            return neighbor;
                        }
                        else {
                            nextfringes.Add(neighbor);
                        }
                        visited.Add(neighbor);
                    }
                }              
                if (distanceDone >= maxDistance && maxDistance != -1) {
                    return null;
                }
            }
            distanceDone++;
            fringes = new List<TileProperties>(nextfringes);
            nextfringes.Clear();
        }
        return null;
    }    
}

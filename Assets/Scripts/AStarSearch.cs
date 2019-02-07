using System.Collections.Generic;
using UnityEngine;

public class AStarSearch { 

    public static double Heuristic(TileProperties a, TileProperties b, Movable movable) {
        return a.Coordinates.Distance(b.Coordinates) * 0.5 ;
    }

    public static double NextCost(TileProperties current, TileProperties next, Movable movable) {
        if (movable && movable.canUseWind && next.wind) { // Free movement if wind
            HexDirection movableDir = current.Coordinates.Direction(next.Coordinates);

            HexDirection beginDir = (current.wind) ? current.wind.direction : movableDir;
            if (current.wind
                && ((beginDir == next.wind.direction && beginDir == movableDir)
                || (beginDir == next.wind.direction.Previous() && next.wind.direction == movableDir)
                || (beginDir == next.wind.direction.Next() && next.wind.direction == movableDir))) {
                return 0;
            }
            /*
            else if (!current.wind && (beginDir == next.wind.direction
                    || beginDir == next.wind.direction.Previous()
                    || beginDir == next.wind.direction.Next())) {
                return 0;
            }*/
            else if (current.wind
                && ((beginDir == next.wind.direction && beginDir == movableDir.Opposite())
                || (beginDir == next.wind.direction.Previous() && next.wind.direction.Previous() == movableDir.Opposite())
                || (beginDir == next.wind.direction.Next() && next.wind.direction.Next() == movableDir.Opposite()))) {
                return next.Tile.walkCost * 1.5;
            }
        }
        return next.Tile.walkCost;
    }

    public static Stack<TileProperties> Path(TileProperties begin, TileProperties end, List<CustomTile> availableTiles = null, Movable movable = null, bool preview = false) {
        if (end == null)
            return null;
        PriorityQueue<TileProperties> frontier = new PriorityQueue<TileProperties>();
        frontier.Enqueue(begin, 0);

        Dictionary<TileProperties, TileProperties> cameFrom = new Dictionary<TileProperties, TileProperties>();
        Dictionary<TileProperties, double> costSoFar = new Dictionary<TileProperties, double>();

        cameFrom.Add(begin, begin);
        costSoFar.Add(begin, 0);

        while (frontier.Count != 0) {
            TileProperties current = frontier.Dequeue();
            if (current == end) {
                break;
            }

            TileProperties[] neighbors = current.GetNeighbors();
            foreach (TileProperties next in neighbors) {
                if (!next || !next.Tile) {
                    continue;
                }
                if (availableTiles != null && !availableTiles.Contains(next.Tile)) {
                    continue;
                }
                if (movable && next != end && ((next.movingEntity && !movable.canPassAboveEntities) || (next.asLake && !movable.canPassAboveLakes) ||
                    ((next.Tile.terrainType == CustomTile.TerrainType.Mountain || next.asMountain) && !movable.canPassAboveMontains) || (next.windOrigin && !movable.canPassAboveWindOrigins))) {
                    continue;
                }
                else if (!movable && ((next.movable && next != end && !preview) || (next.movablePreview && next != end && preview) || next.asLake || next.asMountain)) {
                    continue;
                }
                double newCost = costSoFar[current] + NextCost(current, next, movable);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, end, movable);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // Convert the cameFrom dictionary to a stack that contains the path;
        Stack<TileProperties> path = new Stack<TileProperties>();
        if (!cameFrom.ContainsKey(end)) {
            return null;
        }
        path.Push(end);
        TileProperties currentTile = cameFrom[end];
        path.Push(currentTile);
        while (currentTile != begin) {
            currentTile = cameFrom[currentTile];
            path.Push(currentTile);
        }

        return path;
    }


}

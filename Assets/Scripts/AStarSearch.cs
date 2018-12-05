﻿using System.Collections.Generic;
using UnityEngine;

public class AStarSearch : MonoBehaviour
{
    public Stack<TileProperties> path;

    public static double Heuristic(TileProperties a, TileProperties b) {
        return a.Coordinates.Distance(b.Coordinates);
    }

    public static double NextCost(TileProperties current, TileProperties next) {
        return next.Tile.walkCost;
    }

    public static Stack<TileProperties> Path(TileProperties begin, TileProperties end, List<CustomTile> availableTiles = null) {
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
                if (next == null || !next.Tile.canWalkThrough || (next.movingEntity && next != end) || (availableTiles != null && !availableTiles.Contains(next.Tile) || next.whirlwind)) {
                    //if (next == null || !next.Tile.canWalkThrough || (next.currentMovable && next != end) || (availableTiles != null && !availableTiles.Contains(next.Tile))) {
                    continue;
                }
                double newCost = costSoFar[current] + NextCost(current, next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, end);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glacier : MonoBehaviour {
    public HexDirection riverDirection;
    public bool riverCounterClockwise;
    public int riverForce = 5;

    void Awake() {
        Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(cellPosition);

        TileProperties tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        tile.asMountain = true;
        HexagonalGrid.Instance.humidity.AddGlacier(tile, this);
    }


    static HexMetrics metrics = null;
    void OnDrawGizmosSelected() {
        if(metrics == null)
            metrics = new HexMetrics(3.25f / 2f, 3.977f / 2f);
        Gizmos.color = new Color(0, 0, 1, 1f);

        Vector3 center = this.transform.position + Neigh(riverCounterClockwise ? riverDirection.Opposite().Previous() : riverDirection);
        
        if (riverCounterClockwise) {
            Gizmos.DrawLine(this.transform.position, this.transform.position + metrics.GetCorner((int)riverDirection.Previous()));
            for (int i = 0; i < riverForce;) {
                Gizmos.DrawLine(center + metrics.GetCorner((int)riverDirection.Next()), center + metrics.GetCorner((int)riverDirection));
                i++;
                if (i >= riverForce) return;
                Gizmos.DrawLine(center + metrics.GetCorner((int)riverDirection), center + metrics.GetCorner((int)riverDirection.Previous()));
                i++;
                center += Neigh(riverDirection.Opposite().Previous());
            }
        } else {
            Gizmos.DrawLine(this.transform.position, this.transform.position + metrics.GetCorner((int)riverDirection.Next().Next()));
            for (int i = 0; i < riverForce;) {
                Gizmos.DrawLine(center + metrics.GetCorner((int)riverDirection), center + metrics.GetCorner((int)riverDirection.Next()));
                i++;
                if (i >= riverForce) return;
                Gizmos.DrawLine(center + metrics.GetCorner((int)riverDirection.Next()), center + metrics.GetCorner((int)riverDirection.Next().Next()));
                i++;
                center += Neigh(riverDirection);
            }
        }
    }

    public Vector3 Neigh(HexDirection h) {
        switch(h) {
            case HexDirection.NW:
                return new Vector3(3.977f, 0, 0);
            case HexDirection.SE:
                return new Vector3(-3.977f, 0, 0);
            case HexDirection.W:
                return new Vector3(3.977f / 2f, 3.25f * 3f / 4f, 0);
            case HexDirection.E:
                return new Vector3(-3.977f / 2f, -3.25f * 3f / 4f, 0);
            case HexDirection.NE:
                return new Vector3(3.977f / 2f, -3.25f * 3f / 4f, 0);
            case HexDirection.SW:
                return new Vector3(-3.977f / 2f, 3.25f * 3f / 4f, 0);
        }
        return new Vector3(0, 0, 0);
    }

}

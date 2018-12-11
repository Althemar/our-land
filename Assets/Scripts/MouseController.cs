using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MouseController : MonoBehaviour
{
    public HexagonalGrid hexGrid;
    public ReachableTilesDisplay reachableTiles;

    public MotherShip motherShip;

    public Material movementOutline;

    private Camera cam;
    private Movable movable;

    List<HexagonsOutline> movementPreviews;
   
    void Start() {
        cam = Camera.main;
        movementPreviews = new List<HexagonsOutline>();
        movable = motherShip.GetComponent<Movable>();
    }

    void Update() {

        if (Input.GetMouseButtonDown(1)) {
            RightClickDown();
        }
        else if (Input.GetMouseButtonUp(1)) {
            RightClickUp();
        }
        else if (Input.GetMouseButtonDown(0)) {
            TileProperties tile = GetTile();
            if (motherShip.tileInRange.Contains(tile)) {
                if (tile.staticEntity && motherShip.RemainingActionPoints > 0) {
                    foreach (KeyValuePair<ResourceType, int> resource in tile.staticEntity.entitySO.resources) {
                        motherShip.inventory.AddItem(resource.Key, resource.Value);
                    }
                    motherShip.RemainingActionPoints--;
                    tile.staticEntity.Kill();
                }
                if (tile.movingEntity && motherShip.RemainingActionPoints > 0) {
                    foreach (KeyValuePair<ResourceType, int> resource in tile.movingEntity.entitySO.resources) {
                        motherShip.inventory.AddItem(resource.Key, resource.Value);
                    }
                    motherShip.RemainingActionPoints--;
                    tile.movingEntity.Kill();
                }
            }
        }
        if (reachableTiles.Displaying) {
            reachableTiles.RefreshPath(GetTile());
        }
    }

    TileProperties GetTile() {
        Vector3Int cellPosition = GetCellPositon();
        TileProperties tile = hexGrid.GetTile(cellPosition);
        return tile;
    }

    Vector3Int GetCellPositon() {
        Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = hexGrid.Tilemap.WorldToCell(worldPosition);
        return cellPosition;
    }

    private void RightClickDown() {
        if (!movable.Moving && TurnManager.Instance.State == TurnManager.TurnState.Player) {
            TileProperties tile = GetTile();

            if (motherShip && motherShip.RemainingActionPoints > 0) {
                motherShip.BeginMove();

                List<TileProperties> reachables = movable.CurrentTile.TilesReachable(motherShip.RemainingActionPoints * motherShip.reach, motherShip.reach);

                List<TileProperties>[] reachablesByReach = new List<TileProperties>[motherShip.RemainingActionPoints];
                for (int i = 0; i < reachablesByReach.Length; i++) {
                    reachablesByReach[i] = new List<TileProperties>();
                }


                for (int i = 1; i < reachables.Count; i++) {
                    for (int j = reachables[i].ActionPointCost - 1; j < reachablesByReach.Length; j++) {
                        reachablesByReach[j].Add(reachables[i]);
                    }
                }

                for (int i = 0; i < reachablesByReach.Length; i++) {
                    HexagonsOutline outline = new GameObject().AddComponent<HexagonsOutline>();
                    MeshRenderer renderer = outline.GetComponent<MeshRenderer>();
                    renderer.material = movementOutline;
                    renderer.material.color = new Color(0, 0, 1f - (1f / (motherShip.RemainingActionPoints) * (i - 1)), 1);

                    movementPreviews.Add(outline);

                    for (int j = 0; j < reachablesByReach[i].Count; j++) {
                        reachablesByReach[i][j].IsInReachables = true;
                    }
                    outline.InitMesh(reachablesByReach[i]);
                }

                for (int i = 0; i < reachables.Count; i++) {
                    reachables[i].IsInReachables = false;
                }
                movable.ReachableTiles = reachables;
                reachableTiles.InitReachableTiles(reachables, tile, movable);
            }

        }
    }

    private void RightClickUp() {
        if (reachableTiles.Displaying) {
            reachableTiles.UndisplayReachables();
            for (int i = 0; i < movementPreviews.Count; i++) {
                Destroy(movementPreviews[i].gameObject);
            }
            movementPreviews.Clear();
            if (movable.ReachableTiles.Contains(GetTile())) {
                movable.MoveToTile(GetTile());
            }
        }
    }
}

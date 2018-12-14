using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public HexagonalGrid hexGrid;
    public ReachableTilesDisplay reachableTiles;

    public MotherShip motherShip;

    public Material movementOutline;
    public EntitiesHarvestableUI entitiesHarvestable;

    private Camera cam;
    private Movable movable;

    private TileProperties current;

    List<HexagonsOutline> movementPreviews;
   
    void Start() {
        cam = Camera.main;
        movementPreviews = new List<HexagonsOutline>();
        movable = motherShip.GetComponent<Movable>();
    }

    void Update() {

        if (GameManager.Instance.GameState != GameState.Playing) {
            return;
        }
        
        if (Input.GetMouseButtonDown(1)) {
            RightClickDown();
        }
        else if (Input.GetMouseButtonUp(1)) {
            RightClickUp();
        }
        else if (Input.GetMouseButtonDown(0)) {
            TileProperties tile = GetTile();

            if (motherShip.TilesInRange.Contains(tile) && entitiesHarvestable.CurrentTile != tile && !entitiesHarvestable.CursorIsOnButton()) {
                entitiesHarvestable.Clear();
                entitiesHarvestable.NewEntitiesToHarvest(tile);
            }
            else if (entitiesHarvestable.Displaying && !entitiesHarvestable.CursorIsOnButton()){
                entitiesHarvestable.Clear();
            }
        } else {
            TileProperties tile = GetTile();
            
            if(EventSystem.current.IsPointerOverGameObject()) {
                if (current)
                    hexGrid.ResetTileColor(current.Coordinates.OffsetCoordinates);
                tile = null;
                current = null;
            }

            if(tile && current != tile) {
                if(current)
                    hexGrid.ResetTileColor(current.Coordinates.OffsetCoordinates);
                hexGrid.SetTileColor(tile.Coordinates.OffsetCoordinates, new Color(1, 0, 0, 0.6f));
                entitiesHarvestable.ShowInfo(tile);
                current = tile;
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
        if (entitiesHarvestable.Displaying) {
            entitiesHarvestable.Clear();
        }
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

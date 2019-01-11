using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public HexagonalGrid hexGrid;

    public MotherShip motherShip;
    
    public EntitiesHarvestableUI entitiesHarvestable;

    private Camera cam;
    private Movable movable;
    private ReachableTilesDisplay reachableTiles;

    private TileProperties current;
   
    void Start() {
        cam = Camera.main;
        movable = motherShip.GetComponent<Movable>();
        reachableTiles = motherShip.GetComponent<ReachableTilesDisplay>();
    }

    void Update() {

        if (GameManager.Instance.GameState != GameState.Playing) {
            return;
        }
        
        if (GameManager.Input.GetMouseButtonDown(1)) {
            RightClickDown();
        }
        else if (GameManager.Input.GetMouseButtonUp(1)) {
            RightClickUp();
        }
        else if (GameManager.Input.GetMouseButtonDown(0)) {
            if (!movable.Moving && TurnManager.Instance.State == TurnManager.TurnState.Player) {
                TileProperties tile = GetTile();

                if (motherShip.TilesInRange.Contains(tile) && entitiesHarvestable.CurrentTile != tile && !entitiesHarvestable.CursorIsOnButton()) {
                    entitiesHarvestable.Clear();
                    entitiesHarvestable.NewEntitiesToHarvest(tile);
                }
                else if (entitiesHarvestable.Displaying && !entitiesHarvestable.CursorIsOnButton()) {
                    entitiesHarvestable.Clear();
                }
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
        if (GameManager.Input.GetMouseButton(1) && reachableTiles.Displaying) {
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
        motherShip.targetTile = null;
        if (entitiesHarvestable.Displaying) {
            entitiesHarvestable.Clear();
        }
        if (!movable.Moving && TurnManager.Instance.State == TurnManager.TurnState.Player) {
            TileProperties tile = GetTile();

            if (motherShip) {
                reachableTiles.InitReachableTiles(tile, movable);
            }
        }
    }

    private void RightClickUp() {
        if (reachableTiles.Displaying) {
            TileProperties targetTile = GetTile();
            if (targetTile.movingEntity || targetTile.ActionPointCost > motherShip.Inventory.GetResource(motherShip.fuelResource) || targetTile == motherShip.Movable.CurrentTile
                || !targetTile.IsWalkable()) {
                targetTile = null;
                reachableTiles.UndisplayReachables();
                
            }
            else {
                motherShip.targetTile = targetTile;
                motherShip.Inventory.AddItem(motherShip.fuelResource, Mathf.Floor(-targetTile.ActionPointCost));
            }
        }
    }
}

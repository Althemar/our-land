﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
    public HexagonalGrid hexGrid;

    public MotherShip motherShip;

    public EntitiesHarvestableUI entitiesHarvestable;

    public CameraControl cameraman;

    public bool harvestMode;
    public bool moveMode;

    private Camera cam;
    private Movable movable;
    private ReachableTilesDisplay reachableTiles;

    private TileProperties current;

    void Start() {
        cam = Camera.main;
        movable = motherShip.GetComponent<Movable>();
        reachableTiles = motherShip.GetComponent<ReachableTilesDisplay>();
    }

    Vector3 viewportOrigin;
    bool drag = false;
    void Update() {

        if (GameManager.Instance.GameState != GameState.Playing) {
            return;
        }


        if (GameManager.Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            viewportOrigin = cam.ScreenToWorldPoint(GameManager.Input.mousePosition);
            drag = true;
        }

        if (GameManager.Input.GetMouseButton(0) && drag) {
            Vector2 drag = viewportOrigin - cam.ScreenToWorldPoint(GameManager.Input.mousePosition);

            cameraman.DragCamera(drag);

            viewportOrigin = cam.ScreenToWorldPoint(GameManager.Input.mousePosition);
        }
        else {
            drag = false;
            Vector2 movementCam = new Vector2(GameManager.Input.GetAxis("Horizontal"), GameManager.Input.GetAxis("Vertical"));

            if (cameraman.enableBorderMovement) {
                if (GameManager.Input.mousePosition.x < 5)
                    movementCam.x -= 0.75f;
                if (GameManager.Input.mousePosition.x > Screen.width - 5)
                    movementCam.x += 0.75f;
                if (GameManager.Input.mousePosition.y < 5)
                    movementCam.y -= 0.75f;
                if (GameManager.Input.mousePosition.y > Screen.height - 5)
                    movementCam.y += 0.75f;
            }

            cameraman.MoveTargetCamera(movementCam.x, movementCam.y);
        }
        cameraman.ChangeZoomCamera(-GameManager.Input.mouseScrollDelta.y * 1.5f);

        if (harvestMode) {
            //cameraman.SetTarget(motherShip.transform.position);
            //cameraman.SetZoomLevel(7f);
            if (GameManager.Input.GetMouseButtonDown(0)) {
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
            }
        }

        if(moveMode) {
            if (GameManager.Input.GetMouseButtonDown(1)) {
                ShowPath();
            }
            else if (GameManager.Input.GetMouseButtonUp(1)) {
                SetTarget();
            }

            if (GameManager.Input.GetMouseButton(1) && reachableTiles.Displaying) {
                reachableTiles.RefreshPath(GetTile());
            }
        }
        
        OutlineHex();
    }

    void OutlineHex() {
        TileProperties tile = GetTile();

        if (EventSystem.current.IsPointerOverGameObject()) {
            if (current)
                hexGrid.ResetTileColor(current.Coordinates.OffsetCoordinates);
            tile = null;
            current = null;
        }

        if (tile && current != tile) {
            if (current)
                hexGrid.ResetTileColor(current.Coordinates.OffsetCoordinates);
            hexGrid.SetTileColor(tile.Coordinates.OffsetCoordinates, new Color(1, 0, 0, 0.6f));
            entitiesHarvestable.ShowInfo(tile);
            current = tile;
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

    private void ShowPath() {
        motherShip.targetTile = null;
        if (entitiesHarvestable.Displaying) {
            entitiesHarvestable.Clear();
        }
        if (!motherShip.OnMove && TurnManager.Instance.State == TurnManager.TurnState.Player) {
            TileProperties tile = GetTile();

            if (motherShip) {
                reachableTiles.InitReachableTiles(tile, movable);
            }
        }
    }

    private void SetTarget() {
        if (reachableTiles.Displaying) {
            TileProperties targetTile = GetTile();
            if (targetTile.movingEntity || targetTile.ActionPointCost > motherShip.Inventory.GetResource(motherShip.fuelResource) || targetTile == motherShip.Movable.CurrentTile
                || !targetTile.IsWalkable()) {
                targetTile = null;
                reachableTiles.UndisplayReachables();
            }
            else {
                motherShip.targetTile = targetTile;
                reachableTiles.ValidReachables();
                motherShip.ClearActiveActionPoints();
            }
        }
    }
}

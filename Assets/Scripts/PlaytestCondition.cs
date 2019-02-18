using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestCondition : MonoBehaviour {

    public int level = 1;
    Vector3Int cellPosition;

    public GameObject victoryPanel;

    void Start() {
        Playtest.TimedLog("BEGIN PLAYTEST " + level);

        GameManager.Instance.motherShip.OnTurnBegin -= GameManager.Instance.CheckDefeat;
        GameManager.Instance.motherShip.OnTurnBegin += CheckPlaytest;

        cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(cellPosition);
    }

    void CheckPlaytest() {
        switch (level) {
            case 1:
                if (HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y)) == GameManager.Instance.motherShip.Movable.CurrentTile) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY TURN " + TurnManager.Instance.TurnCount);
                }
                break;
            case 2:
                if (HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y)).staticEntity == null) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY TURN " + TurnManager.Instance.TurnCount);
                }
                break;
            case 3:
                if (HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y)).staticEntity == null) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY TURN " + TurnManager.Instance.TurnCount);
                }
                break;
            case 4:
                if (GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.fuelResource) == 11) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY TURN " + TurnManager.Instance.TurnCount);
                }
                else {
                    GameManager.Instance.CheckDefeat();
                }
                break;
            case 5:
                if (GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.fuelResource) > 30) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY TURN " + TurnManager.Instance.TurnCount);
                }
                else {
                    GameManager.Instance.CheckDefeat();
                }
                break;
            case 6:
                if (GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.foodResource) == 17 &&
                    GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.fuelResource) == 17) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY TURN " + TurnManager.Instance.TurnCount);
                }
                else {
                    GameManager.Instance.CheckDefeat();
                }
                break;
            case 7:
                break;
            default:
                if (GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.fuelResource) > 30) {
                    victoryPanel.SetActive(true);
                    Playtest.TimedLog("VICTORY");
                }
                else {
                    GameManager.Instance.CheckDefeat();
                }
                break;
        }
    }
}

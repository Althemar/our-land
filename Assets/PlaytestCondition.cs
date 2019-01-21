using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestCondition : MonoBehaviour {

    public int level = 1;
    Vector3Int cellPosition;

    public GameObject victoryPanel;

    void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            GameManager.Instance.motherShip.OnTurnBegin -= GameManager.Instance.CheckDefeat;
            GameManager.Instance.motherShip.OnTurnBegin += CheckPlaytest;

            cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(cellPosition);
        }
    }
    
    void CheckPlaytest() {
        switch(level) {
            case 1:
                if(HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y)) == GameManager.Instance.motherShip.Movable.CurrentTile)
                    victoryPanel.SetActive(true);
                break;
            case 2:
                if (HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y)).staticEntity == null)
                    victoryPanel.SetActive(true);
                break;
            case 3:
                if (HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y)).staticEntity == null)
                    Debug.Log("PLAYTEST DONE");
                break;
            case 4:
                if (GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.fuelResource) == 11) {
                    victoryPanel.SetActive(true);
                } else {
                    GameManager.Instance.CheckDefeat();
                }
                break;
            case 5:
                if (GameManager.Instance.motherShip.Inventory.GetResource(GameManager.Instance.motherShip.fuelResource) > 30) {
                    victoryPanel.SetActive(true);
                } else {
                    GameManager.Instance.CheckDefeat();
                }
                break;
            default:
                Debug.LogWarning("Playtest condition not defined");
                break;
        }
    }
}

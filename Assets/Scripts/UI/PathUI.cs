using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathUI : MonoBehaviour
{

    public MotherShip motherShip;
    public Vector3[] pathPoints;
    public TileProperties[] pathTiles;

    public Image templatePoint;
    public Image templateLine;
    
    public Sprite[] randomPath;

    int sizePool = 50;
    CirclePath[] poolPoint;
    Image[] poolLine;

    void Awake() {
        poolPoint = new CirclePath[sizePool];
        poolLine = new Image[sizePool];

        Transform pointsTransform = transform.GetChild(1).transform;
        Transform linesTransform = transform.GetChild(0).transform;
        for(int i = 0; i < sizePool; i++) {
            poolPoint[i] = Instantiate(templatePoint, Vector3.zero, Quaternion.identity, pointsTransform).GetComponent<CirclePath>();
            poolLine[i] = Instantiate(templateLine, Vector3.zero, Quaternion.identity, linesTransform);
            poolPoint[i].gameObject.SetActive(false);
            poolLine[i].gameObject.SetActive(false);
            poolPoint[i].motherShip = motherShip;
        }
    }

    public void UpdatePath() {
        for(int i = 0; i < sizePool; i++) {
            poolPoint[i].gameObject.SetActive(false);
            poolLine[i].gameObject.SetActive(false);
        }

        for(int i = pathPoints.Length - 1; i >= 0; i--) {
            poolPoint[i].transform.position = pathPoints[i];

            TMP_Text text = poolPoint[i].transform.GetChild(0).GetComponent<TMP_Text>();

            if (i >= pathPoints.Length - 2) {
                pathTiles[i].ActionPointCost = 0;
                //text.text = ""
                text.color = Color.black;
            }
            else {
                if (i == pathPoints.Length - 3) {
                    if (FreeWindMovement(pathTiles[i+1], pathTiles[i])){
                        pathTiles[i].ActionPointCost = pathTiles[i + 1].ActionPointCost;
                    }
                    else {
                        pathTiles[i].ActionPointCost = motherShip.movementBaseCost;
                    }
                    text.text = "";
                }
                else {
                    if (FreeWindMovement(pathTiles[i + 1], pathTiles[i])) {
                        pathTiles[i].ActionPointCost = pathTiles[i + 1].ActionPointCost;
                    }
                    else {
                        if (pathTiles[i + 1].ActionPointCost > 0) {
                            pathTiles[i].ActionPointCost = Mathf.Round(pathTiles[i + 1].ActionPointCost * motherShip.movementDistanceMultiplicator);
                        }
                        else {
                            pathTiles[i].ActionPointCost = motherShip.movementBaseCost;
                        }
                    }
                    text.text = Mathf.Floor(pathTiles[i].ActionPointCost).ToString();
                }
                if (pathTiles[i].ActionPointCost > motherShip.Inventory.GetResource(motherShip.fuelResource)) {
                    text.color = Color.red;
                }
                else {
                    text.color = Color.black;
                }
            }
            poolPoint[i].InitCirclePath();
            if (i == 0) {
                if(pathTiles[i].IsWalkable() && !pathTiles[i].asLake && !pathTiles[i].windOrigin && !pathTiles[i].movingEntity && !pathTiles[i].staticEntity && pathTiles[i].Tile.terrainType != CustomTile.TerrainType.Water) {
                    text.text = Mathf.Floor(pathTiles[i].ActionPointCost).ToString() + "<sprite=0>";
                    poolPoint[i].removeImage.gameObject.SetActive(false);
                }
                else {
                    text.text = "";
                    poolPoint[i].removeImage.gameObject.SetActive(true);
                }
                poolPoint[i].SetDestinationSprite();
            }
            else {
                text.text = "";
                poolPoint[i].removeImage.gameObject.SetActive(false);
                poolPoint[i].SetJonctionSprite();
            }
            //text.text = Mathf.Floor(pathTiles[i].ActionPointCost).ToString() + "<sprite=0>";


            poolPoint[i].GetComponent<Image>().raycastTarget = false;
            poolPoint[i].interactable = false;


            poolPoint[i].gameObject.SetActive(true);

            if(i > 0) {
                poolLine[i].rectTransform.position = pathPoints[i];
                Vector3 delta = pathPoints[i - 1] - pathPoints[i];
                poolLine[i].rectTransform.sizeDelta = new Vector3(delta.magnitude, 0.245f);
                poolLine[i].rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
                poolLine[i].sprite = randomPath[Random.Range(0, randomPath.Length)];

                poolLine[i].gameObject.SetActive(true);
            }
        }
    }

    public void SetInteractable() {
        poolPoint[0].GetComponent<Image>().raycastTarget = true;
        poolPoint[0].interactable = true;
        poolPoint[0].SetDestinationSprite();
    }

    private bool FreeWindMovement(TileProperties current, TileProperties next) {
        if (next.wind && motherShip.Movable.canUseWind) { // Free movement if wind
            HexDirection movableDir = current.Coordinates.Direction(next.Coordinates);

            HexDirection beginDir = (current.wind) ? current.wind.direction : movableDir;
            if (current.wind
                && ((beginDir == next.wind.direction && beginDir == movableDir)
                || (beginDir == next.wind.direction.Previous() && next.wind.direction == movableDir)
                || (beginDir == next.wind.direction.Next() && next.wind.direction == movableDir))) {
                return true;
            }
            else if (!current.wind && (beginDir == next.wind.direction
                    || beginDir == next.wind.direction.Previous()
                    || beginDir == next.wind.direction.Next())) {
                return false;
            }
        }
        return false;
    }
    /*
    private bool WindMalusMovement(TileProperties current, TileProperties next) {
        if (next.wind) { // Free movement if wind
            HexDirection movableDir = current.Coordinates.Direction(next.Coordinates);

            HexDirection beginDir = (current.wind) ? current.wind.direction : movableDir;

            if (current.wind
                && ((beginDir == next.wind.direction && beginDir == movableDir.Opposite())
                || (beginDir == next.wind.direction.Previous() && next.wind.direction.Previous() == movableDir.Opposite())
                || (beginDir == next.wind.direction.Next() && next.wind.direction.Next() == movableDir.Opposite()))) {
                return true;
            }
        }
        return false;
    }*/
}

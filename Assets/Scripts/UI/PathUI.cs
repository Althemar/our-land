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

    int sizePool = 20;
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
                    pathTiles[i].ActionPointCost = motherShip.movementBaseCost;
                    //text.text = "";
                }
                else {
                    pathTiles[i].ActionPointCost = pathTiles[i + 1].ActionPointCost * motherShip.movementDistanceMultiplicator;
                    //text.text = Mathf.Floor(pathTiles[i].ActionPointCost).ToString();
                }
                if (pathTiles[i].ActionPointCost > motherShip.Inventory.GetResource(motherShip.fuelResource)) {
                    text.color = Color.red;
                }
                else {
                    text.color = Color.black;
                }
            }
            if (i == 0 && pathTiles[i].IsWalkable()) {
                text.text = Mathf.Floor(pathTiles[i].ActionPointCost).ToString();
                poolPoint[i].interactable = true;
            }
            else {
                text.text = "";
                poolPoint[i].interactable = false;
            }
            poolPoint[i].InitCirclePath();

            poolPoint[i].gameObject.SetActive(true);

            if(i > 0) {
                poolLine[i].rectTransform.position = pathPoints[i];
                Vector3 delta = pathPoints[i - 1] - pathPoints[i];
                poolLine[i].rectTransform.sizeDelta = new Vector3(delta.magnitude, 0.245f);
                poolLine[i].rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
                
                poolLine[i].gameObject.SetActive(true);
            }
        }
    }
}

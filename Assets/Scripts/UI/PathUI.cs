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
    Image[] poolPoint, poolLine;

    void Awake() {
        poolPoint = new Image[sizePool];
        poolLine = new Image[sizePool];

        Transform pointsTransform = transform.GetChild(1).transform;
        Transform linesTransform = transform.GetChild(0).transform;
        for(int i = 0; i < sizePool; i++) {
            poolPoint[i] = Instantiate(templatePoint, Vector3.zero, Quaternion.identity, pointsTransform);
            poolLine[i] = Instantiate(templateLine, Vector3.zero, Quaternion.identity, linesTransform);
            poolPoint[i].gameObject.SetActive(false);
            poolLine[i].gameObject.SetActive(false);
        }
    }

    public void UpdatePath() {
        for(int i = 0; i < sizePool; i++) {
            poolPoint[i].gameObject.SetActive(false);
            poolLine[i].gameObject.SetActive(false);
        }

        for(int i = 0; i < pathPoints.Length; i++) {
            poolPoint[i].transform.position = pathPoints[i];
       
           if (i == 0) {
               // poolPoint[i].transform.GetChild(0).GetComponent<TMP_Text>().text = motherShip.movementBaseCost ;
           }
            else {
                poolPoint[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            }

            poolPoint[i].gameObject.SetActive(true);

            if(i + 1 < pathPoints.Length) {
                poolLine[i].rectTransform.position = pathPoints[i];
                Vector3 delta = pathPoints[i + 1] - pathPoints[i];
                poolLine[i].rectTransform.sizeDelta = new Vector3(delta.magnitude, 0.245f);
                poolLine[i].rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
                
                poolLine[i].gameObject.SetActive(true);
            }
        }
    }
}

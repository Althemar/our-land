using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCamera : MonoBehaviour
{
    public CameraControl cam;
    
    private Queue<Vector3> targetPositions;

    private void Awake() {
        targetPositions = new Queue<Vector3>();
    }

    private void Start() {
        cam.OnReachTarget += GoToTile;
    }

    public void AddTargetPosition(Vector3 position) {
        targetPositions.Enqueue(position);
    }

    public void SetTargetPositions(Mission mission) {
        targetPositions.Clear();
        foreach (Objective objective in mission.missionObjectives) {
            if (objective.targetWithCamera) {
                AddTargetPosition(objective.transform.position);
            }
        }
    }

    public void ReturnToShip(Mission mission) {
        cam.SetTarget(GameManager.Instance.motherShip.transform.position);
    }

    public void GoToTile() {
        if (targetPositions.Count > 0) {
            StartCoroutine(GoToTileCoroutine());
        }
    }

    public IEnumerator GoToTileCoroutine() {
        yield return new WaitForSeconds(2);
        cam.SetTarget(targetPositions.Dequeue(), true); 
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public CameraControl cam;
    public Mission startingMission;
    public NewMissionUI newMissionUI;
    public EndMissionUI endMissionUI;

    public Transform newMissionUIParent;

    private Mission mainMission;
    private Queue<Vector3> targetPositions;

    public static MissionManager Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
        }
    }

    private void Start() {
        GameManager.Instance.motherShip.OnTurnBegin += Evaluate;
        cam.OnReachTarget += GoToTile;
        targetPositions = new Queue<Vector3>();
        StartMission(startingMission);
    }

    private void Evaluate() {
        if (mainMission && mainMission.Evaluate()) {
            Instantiate(endMissionUI, newMissionUIParent).Initialize(mainMission);
        }
    }

    public void StartMission(Mission mission) {
        mainMission = mission;
        mission.StartMission();
        AddTargetPosition(GameManager.Instance.motherShip.transform.position);
        Instantiate(newMissionUI, newMissionUIParent).Initialize(mission);
        GoToTile();
    }

    public void EndMission(Mission mission) {
        if (mission.nextMission) {
            StartMission(mission.nextMission);
        }
        Destroy(mission.gameObject);
    }

    public void AddTargetPosition(Vector3 position) {
        targetPositions.Enqueue(position);
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

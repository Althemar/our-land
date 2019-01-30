using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Mission startingMission;
    public MissionCamera missionCamera;
    public NewMissionUI newMissionUI;
    public EndMissionUI endMissionUI;



    public Transform newMissionUIParent;

    private Mission mainMission;

    public static MissionManager Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
        }
    }

    private void Start() {
        GameManager.Instance.motherShip.OnTurnBegin += Evaluate;
        
        
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
        Instantiate(newMissionUI, newMissionUIParent).Initialize(mission);
        missionCamera.SetTargetPositions(mission);
        missionCamera.GoToTile();
    }

    public void EndMission(Mission mission) {
        if (mission.nextMission) {
            StartMission(mission.nextMission);
        }
        Destroy(mission.gameObject);
    }

 
    
}

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

    private List<Mission> currentMissions;
    private Mission previousMission;
    private int nextMissionIndex;

    public static MissionManager Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
            currentMissions = new List<Mission>();
        }
    }

    private void Start() {
        GameManager.Instance.motherShip.OnTurnBegin += Evaluate;  
        StartMission(startingMission);
    }

    private void Evaluate() {
        foreach (Mission mission in currentMissions) {
            if (mission.Evaluate()) {
                Instantiate(endMissionUI, newMissionUIParent).Initialize(mission);
            }
        }
    }

    public void StartMission(Mission mission) {
        currentMissions.Add(mission);
        mission.StartMission();
        Instantiate(newMissionUI, newMissionUIParent).Initialize(mission);
        missionCamera.SetTargetPositions(mission);
        missionCamera.GoToTile();
    }

    public void AcceptMission() {
        if (previousMission) {
            if (nextMissionIndex < previousMission.nextMission.Length) {
                DisplayNextMission();
            }
            else {
                Destroy(previousMission);
            }
        }
    }

    private void DisplayNextMission() {
        StartMission(previousMission.nextMission[nextMissionIndex]);
        nextMissionIndex++;
    }
    

    public void EndMission(Mission mission) {
        previousMission = mission;
        nextMissionIndex = 0;
        DisplayNextMission();
    }

 
    
}

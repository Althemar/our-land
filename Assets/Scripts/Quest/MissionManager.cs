using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MissionCamera))]
public class MissionManager : MonoBehaviour
{
    public Mission startingMission;
    public MissionCamera missionCamera;
    public NewMissionUI newMissionUI;
    public EndMissionUI endMissionUI;
    public FailedMissionUI failedMissionUI;

    public Transform newMissionUIParent;
    public MissionsUI missionsProgressUI;

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
            else if (mission.failed) {
                Instantiate(failedMissionUI, newMissionUIParent).Initialize(mission);
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

    public void AcceptMission(Mission mission) {
        missionsProgressUI.AddMission(mission);
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

        if (!mission.failed) {
            mission.eventOnSuccess?.Invoke();

            currentMissions.Remove(mission);
            missionsProgressUI.EndMission(mission);
            if (mission.nextMission.Length > 0) {
                previousMission = mission;
                nextMissionIndex = 0;
                DisplayNextMission();
            }
            else {
                Destroy(mission);
            }
        }
        else {
            GameManager.Instance.Defeat();
        }
        
    }

 
    
}

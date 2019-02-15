using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MissionCamera))]
public class MissionManager : Updatable
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
    private Mission newMission;

    public static MissionManager Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
            currentMissions = new List<Mission>();
        }
    }

    private void Start() {
        AddToTurnManager();
        GameManager.Instance.motherShip.OnTurnBegin += ShowNewMission;
        StartMission(startingMission);
    }

    void OnDestroy() {
        RemoveFromTurnManager();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    
    public override void UpdateTurn() {
        base.UpdateTurn();
        
        bool end = true;
        foreach (Mission mission in currentMissions) {
            if (mission.Evaluate()) {
                Instantiate(endMissionUI, newMissionUIParent).Initialize(mission);
                end = false;
            }
            else if (mission.failed) {
                Instantiate(failedMissionUI, newMissionUIParent).Initialize(mission);
                end = false;
            }
        }

        if(end)
            EndTurn();
    }

    public void ShowNewMission() {
        if(!newMission)
            return;
        StartMission(newMission);
        nextMissionIndex++;
        newMission = null;
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
        newMission = previousMission.nextMission[nextMissionIndex];
    }
    

    public void EndMission(Mission mission) {
        if (!mission.failed) {
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
            
            EndTurn();
        }
        else {
            GameManager.Instance.Defeat();
        }
        
    }

 
    
}

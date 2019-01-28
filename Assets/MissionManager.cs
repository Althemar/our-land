using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Mission startingMission;
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
        StartMission(startingMission);
        GameManager.Instance.motherShip.OnTurnBegin += Evaluate;
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
    }

    public void EndMission(Mission mission) {
        if (mission.nextMission) {
            StartMission(mission.nextMission);
        }
        Destroy(mission.gameObject);
    }
}

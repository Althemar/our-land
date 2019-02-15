using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewMissionUI : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text objectives;
    public TMP_Text lore;

    private Mission mission;

    public void Initialize(Mission mission) {
        this.mission = mission;
        title.text = mission.title;
        objectives.text = "";

        objectives.text += mission.mainObjectives.description + "\n";
        if(mission.secondaryObjectives)
            objectives.text += mission.secondaryObjectives.description + "\n";

        lore.text = mission.lore;
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);

    }

    public void AcceptMission() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
        MissionManager.Instance.missionCamera.ReturnToShip(mission);
        MissionManager.Instance.AcceptMission(mission);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FailedMissionUI : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text lore;

    private Mission mission;

    public void Initialize(Mission mission) {
        this.mission = mission;
        title.text = mission.title;
        lore.text = mission.accomplishedLore;

        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);
    }

    public void EndMission() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
        MissionManager.Instance.EndMission(mission);
        Destroy(gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndMissionUI : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text lore;
    public TMP_Text awards;

    private Mission mission;

    public void Initialize(Mission mission) {
        this.mission = mission;
        title.text = mission.title;
        lore.text = mission.accomplishedLore;
        awards.text = mission.awards;

        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);
    }

    public void EndMission() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
        MissionManager.Instance.EndMission(mission);
        Destroy(gameObject);
    }

    
}

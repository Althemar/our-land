using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndMissionUI : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text lore;
    public TMP_Text rewards;

    private Mission mission;

    public void Initialize(Mission mission) {
        this.mission = mission;
        title.text = mission.title;
        lore.text = mission.accomplishedLore;

        rewards.text = "You earned:";
        if (mission.mainObjectives.Completed) {
            foreach (Reward reward in mission.mainObjectives.rewards) {
                rewards.text += "\n" + reward.Display();
                reward.GetReward();
            }
        }
        if (mission.secondaryObjectives && mission.secondaryObjectives.Completed) {
            foreach (Reward reward in mission.secondaryObjectives.rewards) {
                rewards.text += "\n" + reward.Display();
                reward.GetReward();
            }
        }

        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);
    }

    public void EndMission() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
        MissionManager.Instance.EndMission(mission);
        Destroy(gameObject);
    }

    
}

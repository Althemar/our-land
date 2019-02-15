using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionRecap : MonoBehaviour {

    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questGoal;
    public Slider completion;
    public TextMeshProUGUI completionText;
    public Image completionIcon;
    public Image rewardIcon;

    private Mission mission;
    private Objective objective;

    public void Initialize(Mission mission, Objective objective) {
        this.mission = mission;
        this.objective = objective;

        questTitle.text = mission.title;
        questGoal.text = "•  " + objective.description;
        
        if (objective.IconCompletion()) {
            completionIcon.sprite = objective.IconCompletion();
            completionIcon.transform.parent.gameObject.SetActive(true);
        }
        else {
            completionIcon.transform.parent.gameObject.SetActive(false);
        }

        if (objective.IconReward()) {
            rewardIcon.sprite = objective.IconReward();
            rewardIcon.transform.parent.gameObject.SetActive(true);
        }
        else {
            rewardIcon.transform.parent.gameObject.SetActive(false);
        }

        objective.OnUpdate += UpdateGoal;
        UpdateGoal();
    }

    private void UpdateGoal() {
        completion.value = (float)objective.Progress() / (float)objective.Goal();
        completionText.text = "<voffset=0.5em>" + objective.Progress() + "</voffset>/<voffset=-0.5em>" + objective.Goal() + "</voffset>";
    }

}

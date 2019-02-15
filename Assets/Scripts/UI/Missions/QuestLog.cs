using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLog : MonoBehaviour {

    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questGoal;
    public Slider completion;
    public TextMeshProUGUI completionText;

    private Mission mission;
    private Objective objective;

    public void Initialize(Mission mission, Objective objective) {
        this.mission = mission;
        this.objective = objective;

        questTitle.text = mission.title;
        questGoal.text = "•  " + objective.description;
        objective.OnUpdate += UpdateGoal;
        UpdateGoal();
    }

    private void UpdateGoal() {
        completion.value = (float)objective.Progress() / (float)objective.Goal();
        completionText.text = "<voffset=0.5em>" + objective.Progress() + "</voffset>/<voffset=-0.5em>" + objective.Goal() + "</voffset>";
    }

}

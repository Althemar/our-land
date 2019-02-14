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

    private Mission mission;
    private int objNum;

    public void Initialize(Mission mission, int objNum) {
        this.mission = mission;
        this.objNum = objNum;

        questTitle.text = mission.title;
        questGoal.text = "•  " + mission.missionObjectives[objNum].description;
        mission.missionObjectives[objNum].OnUpdate += UpdateGoal;
        UpdateGoal();
    }

    private void UpdateGoal() {
        completion.value = (float)mission.missionObjectives[objNum].Progress() / (float)mission.missionObjectives[objNum].Goal();
        completionText.text = "<voffset=0.5em>" + mission.missionObjectives[objNum].Progress() + "</voffset>/<voffset=-0.5em>" + mission.missionObjectives[objNum].Goal() + "</voffset>";
    }

}

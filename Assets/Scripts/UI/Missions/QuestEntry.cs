using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestEntry : MonoBehaviour {
    public TextMeshProUGUI questTitle;
    public GameObject secondaryQuest;
    public TextMeshProUGUI secondTitle;

    private Mission mission;

    public void Initialize(Mission mission) {
        this.mission = mission;

        questTitle.text = mission.title;
        if(mission.secondaryObjectives) {
            secondaryQuest.SetActive(true);
            secondTitle.text = mission.secondaryObjectives.description;
        } else {
            secondaryQuest.SetActive(false);
        }
    }

}

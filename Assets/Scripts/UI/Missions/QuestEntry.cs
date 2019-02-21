using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestEntry : MonoBehaviour {
    public TextMeshProUGUI questTitle;

    private Mission mission;
    private QuestLog log;

    public void Initialize(Mission mission, QuestLog log) {
        this.mission = mission;
        this.log = log;

        questTitle.text = mission.title;

        GetComponent<Button>().onClick.AddListener(UpdateLog);
    }

    public void UpdateLog() {
        AkSoundEngine.PostEvent("Play_Choice_Pl", this.gameObject);
        log.Initialize(mission);
    }

}

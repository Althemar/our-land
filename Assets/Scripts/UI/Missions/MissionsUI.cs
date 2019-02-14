using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsUI : MonoBehaviour
{
    //public MissionUI missionUIPrefab;

    //private Dictionary<Mission, MissionUI> missionUIs;
    
    public MissionRecap currentMission;

    private void Awake() {
        //missionUIs = new Dictionary<Mission, MissionUI>();
        //image = GetComponent<Image>();
    }
    /*
    private void Start() {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }*/

    public void AddMission(Mission mission) {
        currentMission.Initialize(mission);
        //missionUIs.Add(mission, missionUI);
        /*if (image.color.a == 0) {
            Color color = image.color;
            color.a = backgroundAlpha;
            image.color = color;
        }*/
    }

    public void EndMission(Mission mission) {
       /* MissionUI missionUI = missionUIs[mission];
        missionUI.DestroyObjectivesUI();
        Destroy(missionUI.gameObject);
        missionUIs.Remove(mission);

        if (missionUIs.Count == 0) {
            Color color = image.color;
            color.a = 0;
            image.color = color;
        }*/
    }
}

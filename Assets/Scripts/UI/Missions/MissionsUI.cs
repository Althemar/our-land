using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsUI : MonoBehaviour
{
    public MissionUI missionUIPrefab;
    public float backgroundAlpha = 0.7f;

    private Dictionary<Mission, MissionUI> missionUIs;
    private Image image;

    private void Awake() {
        missionUIs = new Dictionary<Mission, MissionUI>();
        image = GetComponent<Image>();
    }

    private void Start() {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }

    public void AddMission(Mission mission) {
        MissionUI missionUI = Instantiate(missionUIPrefab, transform).GetComponent<MissionUI>();
        missionUI.Initialize(mission);
        missionUIs.Add(mission, missionUI);
        if (image.color.a == 0) {
            Color color = image.color;
            color.a = backgroundAlpha;
            image.color = color;
        }
    }

    public void EndMission(Mission mission) {
        MissionUI missionUI = missionUIs[mission];
        missionUI.DestroyObjectivesUI();
        Destroy(missionUI.gameObject);
        missionUIs.Remove(mission);

        if (missionUIs.Count == 0) {
            Color color = image.color;
            color.a = 0;
            image.color = color;
        }
    }

    public void UpdateMissionsUI() {
        foreach (KeyValuePair<Mission, MissionUI> pair in missionUIs) {
            pair.Value.UpdateMissionUI();
        }
    }

}

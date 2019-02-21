using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsUI : MonoBehaviour {
    public MissionRecap currentMission;
    public MissionRecap secondaryMission;

    public QuestMenu questPanel;

    RectTransform missionTransform, secondaryTransform;
    Vector3 beginPosition, beginPositionSecondary;

    void Start() {
        missionTransform = currentMission.GetComponent<RectTransform>();
        beginPosition = missionTransform.anchoredPosition;
        missionTransform.anchoredPosition = new Vector3(missionTransform.rect.width + 50, beginPosition.y, beginPosition.z);

        secondaryTransform = secondaryMission.GetComponent<RectTransform>();
        beginPositionSecondary = secondaryTransform.anchoredPosition;
        secondaryTransform.anchoredPosition = new Vector3(secondaryTransform.rect.width + 150, beginPositionSecondary.y, beginPositionSecondary.z);

        
    }

    public void AddMission(Mission mission) {
        StopAllCoroutines();
        currentMission.Initialize(mission, mission.mainObjectives);
        StartCoroutine(Open());

        if (mission.secondaryObjectives) {
            secondaryMission.Initialize(mission, mission.secondaryObjectives);
            StartCoroutine(OpenSecondary());
        }

        questPanel.AddMission(mission);
    }

    public void EndMission(Mission mission) {
        StopAllCoroutines();
        StartCoroutine(Close());
        StartCoroutine(CloseSecondary());
    }


    IEnumerator Open() {
        float progress = 0;
        Vector3 currentPos = missionTransform.anchoredPosition;
        while (progress <= 1) {
            missionTransform.anchoredPosition = Vector3.Lerp(currentPos, new Vector3(0, beginPosition.y, beginPosition.z), progress);
            progress += Time.deltaTime * 5;
            yield return null;
        }
        missionTransform.anchoredPosition = new Vector3(0, beginPosition.y, beginPosition.z);
    }

    IEnumerator OpenSecondary() {
        float progress = 0;
        Vector3 currentPos = secondaryTransform.anchoredPosition;
        while (progress <= 1) {
            secondaryTransform.anchoredPosition = Vector3.Lerp(currentPos, new Vector3(0, beginPositionSecondary.y, beginPositionSecondary.z), progress);
            progress += Time.deltaTime * 5;
            yield return null;
        }
        secondaryTransform.anchoredPosition = new Vector3(0, beginPositionSecondary.y, beginPositionSecondary.z);
    }

    IEnumerator Close() {
        float progress = 0;
        Vector3 currentPos = missionTransform.anchoredPosition;
        while (progress <= 1) {
            missionTransform.anchoredPosition = Vector3.Lerp(currentPos, new Vector3(missionTransform.rect.width + 50, beginPosition.y, beginPosition.z), progress);
            progress += Time.deltaTime * 5;
            yield return null;
        }
        missionTransform.anchoredPosition = new Vector3(missionTransform.rect.width + 50, beginPosition.y, beginPosition.z);
    }

    IEnumerator CloseSecondary() {
        float progress = 0;
        Vector3 currentPos = secondaryTransform.anchoredPosition;
        while (progress <= 1) {
            secondaryTransform.anchoredPosition = Vector3.Lerp(currentPos, new Vector3(secondaryTransform.rect.width + 150, beginPositionSecondary.y, beginPositionSecondary.z), progress);
            progress += Time.deltaTime * 5;
            yield return null;
        }
        secondaryTransform.anchoredPosition = new Vector3(secondaryTransform.rect.width + 150, beginPositionSecondary.y, beginPositionSecondary.z);
    }

}

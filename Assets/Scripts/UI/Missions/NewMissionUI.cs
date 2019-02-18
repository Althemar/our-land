using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewMissionUI : MonoBehaviour {
    public TMP_Text title;
    public TMP_Text lore;


    public TMP_Text mainGoalText;
    public Slider mainCompletion;
    public TMP_Text mainCompletionText;
    public Image mainCompletionIcon;
    public GameObject mainReward;
    public Image mainRewardIcon;

    public GameObject secondaryGoal;
    public TMP_Text secondaryGoalText;
    public Slider secondaryCompletion;
    public TMP_Text secondaryCompletionText;
    public Image secondaryCompletionIcon;
    public GameObject secondaryReward;
    public Image secondaryRewardIcon;

    private Mission mission;

    public void Initialize(Mission mission) {
        AkSoundEngine.PostEvent("Play_SFX_Quest_New", this.gameObject);

        this.mission = mission;
        title.text = mission.title;
        mainGoalText.text = "•  " + mission.mainObjectives.description;
        mainCompletion.value = (float)mission.mainObjectives.Progress() / (float)mission.mainObjectives.Goal();
        mainCompletionText.text = "<voffset=0.5em>" + mission.mainObjectives.Progress() + "</voffset>/<voffset=-0.5em>" + mission.mainObjectives.Goal() + "</voffset>";

        if (mission.mainObjectives.IconCompletion()) {
            mainCompletionIcon.sprite = mission.mainObjectives.IconCompletion();
            mainCompletionIcon.transform.parent.gameObject.SetActive(true);
        }
        else {
            mainCompletionIcon.transform.parent.gameObject.SetActive(false);
        }

        if (mission.mainObjectives.IconReward()) {
            mainRewardIcon.sprite = mission.mainObjectives.IconReward();
            mainReward.SetActive(true);
        }
        else {
            mainReward.SetActive(false);
        }

        if (mission.secondaryObjectives) {
            secondaryGoal.SetActive(true);
            secondaryGoalText.text = "•  " + mission.secondaryObjectives.description;
            secondaryCompletion.value = (float)mission.secondaryObjectives.Progress() / (float)mission.secondaryObjectives.Goal();
            secondaryCompletionText.text = "<voffset=0.5em>" + mission.secondaryObjectives.Progress() + "</voffset>/<voffset=-0.5em>" + mission.secondaryObjectives.Goal() + "</voffset>";

            if (mission.secondaryObjectives.IconCompletion()) {
                secondaryCompletionIcon.sprite = mission.secondaryObjectives.IconCompletion();
                secondaryCompletionIcon.transform.parent.gameObject.SetActive(true);
            }
            else {
                secondaryCompletionIcon.transform.parent.gameObject.SetActive(false);
            }

            if (mission.secondaryObjectives.IconReward()) {
                secondaryRewardIcon.sprite = mission.secondaryObjectives.IconReward();
                secondaryReward.SetActive(true);
            }
            else {
                secondaryReward.SetActive(false);
            }
        }
        else {
            secondaryGoal.SetActive(false);
        }

        lore.text = mission.lore;
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);
    }

    public void AcceptMission() {
        AkSoundEngine.PostEvent("Play_Choice_Pl", this.gameObject);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
        MissionManager.Instance.missionCamera.ReturnToShip(mission);
        MissionManager.Instance.AcceptMission(mission);
        Destroy(gameObject);
    }
}

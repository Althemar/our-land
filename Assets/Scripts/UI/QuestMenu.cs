using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : MonoBehaviour {
    public CanvasReference canvasRef;

    public GameObject questMenu;
    public ShipMenu ship;
    public GameObject containerList;
    public GameObject entry;
    public QuestLog log;
    private List<Mission> missionLog = new List<Mission>();

    RectTransform menu;
    Vector3 beginPosition;
    
    bool isOpen;

    private void Start() {
        isOpen = false;
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Quest, false);
        questMenu.SetActive(false);

        menu = GetComponent<RectTransform>();
        beginPosition = menu.anchoredPosition;
    }

    public void AddMission(Mission mission) {
        missionLog.Insert(0, mission);
        GameObject entryObject = Instantiate(entry, Vector3.zero, Quaternion.identity, containerList.transform);
        entryObject.GetComponent<QuestEntry>().Initialize(mission, log);
        entryObject.transform.SetAsFirstSibling();
    }

    public void Toogle() {
        isOpen ^= true;

        ship.CloseMenu();

        if (isOpen) {
            AkSoundEngine.PostEvent("Play_SFX_Button_IGMenu_Open", this.gameObject);
            AkSoundEngine.PostEvent("Play_SFX_Button_YourQuests_Open", this.gameObject);
        }
        else {
            AkSoundEngine.PostEvent("Play_SFX_Button_IGMenu_Close", this.gameObject);
            AkSoundEngine.PostEvent("Play_SFX_Button_YourQuests_Close", this.gameObject);
        }

        StopAllCoroutines();
        StartCoroutine(isOpen ? Open() : Close());
    }

    public void CloseMenu() {
        if (!isOpen)
            return;

        isOpen = false;

        AkSoundEngine.PostEvent("Play_SFX_Button_IGMenu_Close", this.gameObject);
        AkSoundEngine.PostEvent("Play_SFX_Button_YourQuests_Close", this.gameObject);

        StopAllCoroutines();
        StartCoroutine(Close());
    }

    IEnumerator Open() {
        questMenu.SetActive(true);
        if(missionLog.Count > 0)
            log.Initialize(missionLog[0]);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Quest, true);

        float progress = 0;
        Vector3 currentPos = menu.anchoredPosition;
        while (progress <= 1) {
            menu.anchoredPosition = Vector3.Lerp(currentPos, new Vector3(-menu.rect.width, beginPosition.y, beginPosition.z), progress);
            progress += Time.deltaTime;
            yield return null;
        }
        menu.anchoredPosition =new Vector3(-menu.rect.width, beginPosition.y, beginPosition.z);
    }

    IEnumerator Close() {
        float progress = 0;
        Vector3 currentPos = menu.anchoredPosition;
        while (progress <= 1) {
            menu.anchoredPosition = Vector3.Lerp(currentPos, beginPosition, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        menu.anchoredPosition = beginPosition;

        questMenu.SetActive(false);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Quest, false);
    }

}

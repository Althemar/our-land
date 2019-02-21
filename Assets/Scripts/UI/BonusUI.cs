using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusUI : MonoBehaviour {

    public Bonus bonus;
    public TextMeshProUGUI progressText;
    public GameObject populationPoints;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Toogle);
    }

    void OnEnable() {
        progressText.text = "Level " + bonus.Level + "\nProgress " + bonus.Progress;
    }

    void Toogle() {
        bonus.ToogleActive();
        if (bonus.IsActive)
            AkSoundEngine.PostEvent("Play_SFX_Button_PPOn", this.gameObject);
        else
            AkSoundEngine.PostEvent("Play_SFX_Button_PPOff", this.gameObject);
        populationPoints.SetActive(bonus.IsActive);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusUI : MonoBehaviour {

    public Bonus bonus;
    public TextMeshProUGUI progressText;
    public Slider progressSlider;
    public GameObject populationPoints;
    
    void Start() {
        GetComponent<Button>().onClick.AddListener(Toogle);

        GameManager.Instance.motherShip.OnTurnBegin += UpdateBonusUI;
    }

    void OnDestroy() {
        GameManager.Instance.motherShip.OnTurnBegin -= UpdateBonusUI;
    }

    void OnEnable() {
        UpdateBonusUI();
    }

    void UpdateBonusUI() {
        progressText.text = "<size=15><b>" + bonus.BonusName() + "</b></size>\n";
        progressText.text += bonus.BonusEffect(1);
        progressText.text += bonus.BonusEffect(2);
        progressText.text += bonus.BonusEffect(3);

        progressSlider.value = bonus.Progress;
    }

    void Toogle() {
        bonus.ToogleActive();

        progressText.text = "<size=15><b>" + bonus.BonusName() + "</b></size>\n";
        progressText.text += bonus.BonusEffect(1);
        progressText.text += bonus.BonusEffect(2);
        progressText.text += bonus.BonusEffect(3);

        progressSlider.value = bonus.Progress;

        if (bonus.IsActive)
            AkSoundEngine.PostEvent("Play_SFX_Button_PPOn", this.gameObject);
        else
            AkSoundEngine.PostEvent("Play_SFX_Button_PPOff", this.gameObject);
        populationPoints.SetActive(bonus.IsActive);
    }

}

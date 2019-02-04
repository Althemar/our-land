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
        populationPoints.SetActive(bonus.IsActive);
    }

}

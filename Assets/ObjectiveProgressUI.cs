using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveProgressUI : MonoBehaviour
{
    public Image completed;
    public TMP_Text text;

    private Objective objective;

    public void Initialize(Objective objective) {
        this.objective = objective;
    }

    public void UpdateText(bool completed) {
        text.text = objective.GetProgressText();

    }
  
}

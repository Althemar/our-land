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
        text.text = objective.GetProgressText();
        objective.OnUpdate += UpdateObjective;
    }

    public void UpdateObjective() {
        text.text = objective.GetProgressText();
        if (objective.Failed) {
            completed.color = Color.red;
        }
        else if (objective.Completed) {
            completed.color = Color.green;
        }
        else {
            completed.color = Color.white;
        }

    }
  
}

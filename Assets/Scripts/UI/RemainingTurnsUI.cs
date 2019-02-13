using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemainingTurnsUI : MonoBehaviour
{
    public TMP_Text turnsText;

    public void UpdateRemainingTurns(int turns) {
        turnsText.text = turns + " turns remaining";
    }
}

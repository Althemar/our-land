using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnCountUI : MonoBehaviour
{
    public TurnManager turnManager;
    public TMP_Text turnCount;

    private void Start() {
        turnManager.OnEndTurn += UdpateTurnCountText;
    }

    private void UdpateTurnCountText()
    {
        turnCount.text = turnManager.TurnCount.ToString();
    }
}

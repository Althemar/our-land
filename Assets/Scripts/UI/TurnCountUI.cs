using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnCountUI : MonoBehaviour {
    public TMP_Text turnCount;

    private void Start() {
        TurnManager.Instance.OnEndTurn += UdpateTurnCountText;
    }

    private void UdpateTurnCountText() {
        turnCount.text = TurnManager.Instance.TurnCount.ToString();
    }
}

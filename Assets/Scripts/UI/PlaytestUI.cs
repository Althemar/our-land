using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestUI : MonoBehaviour
{
    void Start() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);
    }

    public void OKButton() {
        Destroy(this.gameObject);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
    }

    public void NextLevel() {
        Destroy(this.gameObject);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
    }
}

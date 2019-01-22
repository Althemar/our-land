using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestUI : MonoBehaviour
{
    void Start() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, true);
        Console.AddCommand("skipLevel", ((string[] arg) => NextLevel()),  "Skip the current playtest level");
    }

    public void OKButton() {
        Destroy(this.gameObject);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);
    }

    public void NextLevel() {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Defeat, false);

        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}

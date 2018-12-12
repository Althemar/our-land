using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Defeat
}

public class GameManager : MonoBehaviour
{
    public MotherShip motherShip;

    public Canvas gameOverPanel;

    public float timeToWaitAfterEnd;
    public int frameCount = 0;

    private GameState gameState;

    public static GameManager Instance;


    public GameState GameState
    {
        get => gameState;
    }


    public int FrameCount
    {
        get => frameCount;
    }

    private void Awake() {
        if (!Instance) {
            Instance = this;
            motherShip.OnTurnBegin += CheckDefeat;
            gameOverPanel.gameObject.SetActive(false);
            gameState = GameState.Playing;
        } 
    }

    public void CheckDefeat() {
        if (motherShip.Inventory.GetResource(motherShip.foodResource) <= 0) {
            gameOverPanel.gameObject.SetActive(true);
            gameState = GameState.Defeat;
            StartCoroutine(WaitBeforeFinish());
        }
    }

    private void Update() {
        if (frameCount == 0) {
            frameCount++;
        }
    }

    public IEnumerator WaitBeforeFinish() {
        yield return new WaitForSeconds(timeToWaitAfterEnd);
        Instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

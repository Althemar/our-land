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

    public DefeatUI gameOverPanel;

    public float timeToWaitAfterEnd;

    private int frameCount = 0;
    private GameState gameState;

    public static GameManager Instance;

    public static class Input {
        public enum Blocker {
            None = 0,
            Console = 1,
            Pause = 2
        }
        static Blocker blocks;

        public static Vector3 mousePosition {
            get {
                return blocks != Blocker.None ? new Vector3(Screen.width, Screen.height) / 2f : UnityEngine.Input.mousePosition;
            }
        }
        public static Vector2 mouseScrollDelta {
            get {
                return blocks != Blocker.None ? Vector2.zero : UnityEngine.Input.mouseScrollDelta;
            }
        }

        public static void SetBlock(Blocker b, bool value) {
            if (value)
                blocks |= b;
            else
                blocks &= ~b;
        }

        internal static float GetAxis(string axis) {
            return blocks != Blocker.None ? 0.0f : UnityEngine.Input.GetAxis(axis);
        }
        internal static float GetAxisRaw(string axis) {
            return blocks != Blocker.None ? 0.0f : UnityEngine.Input.GetAxisRaw(axis);
        }

        internal static bool GetButton(string but) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetButton(but);
        }
        internal static bool GetButtonUp(string but) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetButtonUp(but);
        }
        internal static bool GetButtonDown(string but) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetButtonDown(but);
        }

        internal static bool GetKey(KeyCode key) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetKey(key);
        }
        internal static bool GetKeyUp(KeyCode key) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetKeyUp(key);
        }
        internal static bool GetKeyDown(KeyCode key) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetKeyDown(key);
        }

        internal static bool GetMouseButton(int button) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetMouseButton(button);
        }
        internal static bool GetMouseButtonUp(int button) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetMouseButtonUp(button);
        }
        internal static bool GetMouseButtonDown(int button) {
            return blocks != Blocker.None ? false : UnityEngine.Input.GetMouseButtonDown(button);
        }
    }

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

        ConfigVar.Init();
        
        if(!Console.isInit) {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("ConsoleGUI"));
            DontDestroyOnLoad(consoleUI);
            Console.Init(consoleUI);
            Console.AddCommand("reset", CmdReset, "Reset the game");
        }
    }

    public void CheckDefeat() {
        if (motherShip.Inventory.GetResource(motherShip.foodResource) <= 0) {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.text.text = "Your people survived " + TurnManager.Instance.TurnCount + " turns";
            gameState = GameState.Defeat;
            Playtest.TimedLog("Defeat");
            StartCoroutine(WaitBeforeFinish());
        }
    }

    private void Update() {
        frameCount++;
        
        Console.ConsoleUpdate();
    }

    private void LateUpdate() {
        Console.ConsoleLateUpdate();
    }

    public IEnumerator WaitBeforeFinish() {
        yield return new WaitForSeconds(timeToWaitAfterEnd);
        ResetGame();
    }

    public void CmdReset(string[] arg) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetGame() {
        Playtest.TimedLog("Game Reset Turn " + TurnManager.Instance.TurnCount);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

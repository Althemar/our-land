using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEngine.Tilemaps;
using RotaryHeart.Lib.SerializableDictionary;
using System;

public enum GameState
{
    Playing,
    Defeat
}



public class GameManager : MonoBehaviour
{
    public MotherShip motherShip;
    public MovingEntity fishPrefab;

    public DefeatUI gameOverPanel;

    public float timeToWaitAfterEnd;

    private int frameCount = 0;
    private GameState gameState;

  
    

    public static GameManager Instance;

    public static class Input {
        public enum Blocker {
            None = 0,
            Console = 1,
            Pause = 2,
            Defeat = 4,
            Ship = 8
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

        public static bool IsBlock { get {
                return blocks != Blocker.None;
            }
        }

        public static void SetBlock(Blocker b, bool value) {
            if (value)
                blocks |= b;
            else
                blocks &= ~b;
        }

        internal static float GetAxis(string axis) {
            return IsBlock ? 0.0f : UnityEngine.Input.GetAxis(axis);
        }
        internal static float GetAxisRaw(string axis) {
            return IsBlock ? 0.0f : UnityEngine.Input.GetAxisRaw(axis);
        }

        internal static bool GetButton(string but) {
            return IsBlock ? false : UnityEngine.Input.GetButton(but);
        }
        internal static bool GetButtonUp(string but) {
            return IsBlock ? false : UnityEngine.Input.GetButtonUp(but);
        }
        internal static bool GetButtonDown(string but) {
            return IsBlock ? false : UnityEngine.Input.GetButtonDown(but);
        }

        internal static bool GetKey(KeyCode key) {
            return IsBlock ? false : UnityEngine.Input.GetKey(key);
        }
        internal static bool GetKeyUp(KeyCode key) {
            return IsBlock ? false : UnityEngine.Input.GetKeyUp(key);
        }
        internal static bool GetKeyDown(KeyCode key) {
            return IsBlock ? false : UnityEngine.Input.GetKeyDown(key);
        }

        internal static bool GetMouseButton(int button) {
            return IsBlock ? false : UnityEngine.Input.GetMouseButton(button);
        }
        internal static bool GetMouseButtonUp(int button) {
            return IsBlock ? false : UnityEngine.Input.GetMouseButtonUp(button);
        }
        internal static bool GetMouseButtonDown(int button) {
            return IsBlock ? false : UnityEngine.Input.GetMouseButtonDown(button);
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
            Input.SetBlock(Input.Blocker.Defeat, false);
            gameState = GameState.Playing;
        }

        ConfigVar.Init();
        
        if(!Console.isInit) {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("ConsoleGUI"));
            DontDestroyOnLoad(consoleUI);
            Console.Init(consoleUI);
        }

        Console.AddCommand("reset", CmdReset, "Reset the game");
        Console.AddCommand("loadScene", CmdLoad, "Load a scene");
    }

    public void CheckDefeat() {
        if (motherShip.foodResource && motherShip.Inventory.GetResource(motherShip.foodResource) <= 0) {
            Defeat();
            
        }
    }

    public void Defeat() {
        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.text.text = "Votre peuple a survécu " + TurnManager.Instance.TurnCount + " tours";
        Input.SetBlock(Input.Blocker.Defeat, true);

        gameState = GameState.Defeat;
        StartCoroutine(WaitBeforeFinish());
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

    public void CmdReset(string[] args) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CmdLoad(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid number");
                return;
            }

            if (n < 0 || n >= SceneManager.sceneCountInBuildSettings) {
                Console.Write("Error: Invalid scene");
                return;
            }

            SceneManager.LoadScene(n);
        }
        else {
            Console.Write("Usage: loadScene [n] \nLoad the n-th scene.");
        }
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour {

    Canvas canvas;
    
    public GameObject optionPrefab;
    GameObject optionInstance = null;

    void Awake () {
        AkSoundEngine.StopAll();
        Playtest.EndPlaytest();

        canvas = GetComponent<Canvas>();

        ConfigVar.Init();
        
        if(!Console.isInit) {
            var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("ConsoleGUI"));
            DontDestroyOnLoad(consoleUI);
            Console.Init(consoleUI);
        } else {
            Console.Reset();
        }
    }
    
    void Update() {
        if(optionInstance != null)
            canvas.enabled = false;
        else
            canvas.enabled = true;

        Console.ConsoleUpdate();
    }

    private void LateUpdate() {
        Console.ConsoleLateUpdate();
    }

    public void ShowOptions() {
        optionInstance = Instantiate(optionPrefab);
    }

    public void Play() {
        SceneManager.LoadScene("LD tests");

        //Playtest.StartNewPlaytest();
    }

}
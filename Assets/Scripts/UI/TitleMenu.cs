using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour {

    Canvas canvas;
    
    public GameObject optionPrefab;
    GameObject optionInstance = null;

    void Awake () {
        canvas = GetComponent<Canvas>();
    }
    
    void Update() {
        if(optionInstance != null)
            canvas.enabled = false;
        else
            canvas.enabled = true;
    }

    public void ShowOptions() {
        optionInstance = Instantiate(optionPrefab);
    }

    public void Play() {
        SceneManager.LoadScene("Playtest");

        Playtest.StartNewPlaytest();
    }

}
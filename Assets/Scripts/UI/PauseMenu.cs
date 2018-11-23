using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionPrefab;
    GameObject optionInstance = null;
    public GameObject buttonsObject;
    
    void Update() {
        if(optionInstance != null)
            buttonsObject.SetActive(false);
        else
            buttonsObject.SetActive(true);
    }

    public void Resume() {
        Destroy(this.gameObject);
    }

    public void ShowOptions() {
        optionInstance = Instantiate(optionPrefab);
    }

    public void Exit() {
        Application.Quit();
    }
}

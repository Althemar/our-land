using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnDestroy() {
        Destroy(optionInstance);
    }

    public void Resume() {
        Camera.main.cullingMask = Camera.main.cullingMask | LayerMask.GetMask("UI");
        Destroy(this.gameObject);
    }

    public void ShowOptions() {
        optionInstance = Instantiate(optionPrefab);
    }

    public void Exit() {
        //Application.Quit();
        Playtest.EndPlaytest();

        SceneManager.LoadScene("Menu");
    }
}

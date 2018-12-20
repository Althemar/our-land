using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

    public GameObject pausePrefab;
    GameObject pauseInstance = null;

    void Update () {
        if(GameManager.Input.GetButtonDown("Cancel")) {
            if(pauseInstance == null) {
                Camera.main.cullingMask = Camera.main.cullingMask & (~LayerMask.GetMask("UI"));
                pauseInstance = Instantiate(pausePrefab);
            }
            else {
                Camera.main.cullingMask = Camera.main.cullingMask | LayerMask.GetMask("UI");
                Destroy(pauseInstance);
            }
        }
    }

}
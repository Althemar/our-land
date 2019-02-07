using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

    public GameObject pausePrefab;
    GameObject pauseInstance = null;

    void Update () {
        if(Input.GetButtonDown("Cancel")) {
            if(pauseInstance == null) {
                Camera.main.GetComponent<DrawEntity>().UICam.cullingMask = Camera.main.GetComponent<DrawEntity>().UICam.cullingMask & (~LayerMask.GetMask("UI"));
                pauseInstance = Instantiate(pausePrefab);
            }
            else {
                Camera.main.GetComponent<DrawEntity>().UICam.cullingMask = Camera.main.GetComponent<DrawEntity>().UICam.cullingMask | LayerMask.GetMask("UI");
                Destroy(pauseInstance);
            }
        }
    }

}
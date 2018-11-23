using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

    public GameObject pausePrefab;
    GameObject pauseInstance = null;

    void Update () {
        if(Input.GetButtonDown("Cancel")) {
            if(pauseInstance == null)
                pauseInstance = Instantiate(pausePrefab);
            else
                Destroy(pauseInstance);
        }
    }

}
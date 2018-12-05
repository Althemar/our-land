using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public GameObject whirlwind;

    public static WindManager Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
}

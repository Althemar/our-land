using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroup : MonoBehaviour
{

    public delegate void OnBird();
    public OnBird OnReachGoal;
    public float speed = 2f;


    private Vector3 spawn;
    private Vector3 goal;

    float progress = 0;

    public void Initialize(Vector3 spawn, Vector3 goal, CameraControl cam) {
        this.spawn = spawn;
        this.goal = goal;
        transform.position = spawn;
        progress = 0;

        foreach (Transform child in transform){
            child.GetComponent<Bird>().Initialize(cam);
        }
    }

    // Update is called once per frame
    void Update()
    {
        progress += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(spawn, goal, progress);
        if (progress >= 1) {
            OnReachGoal?.Invoke();
            Destroy(gameObject);
        }
    }
}

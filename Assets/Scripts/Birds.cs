using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour
{
    public BirdGroup birdPrefab;

    public Transform spawn;
    public Transform goal;

    public CameraControl cam;
    private Color tmpColor;

    // Start is called before the first frame update
    void Start()
    {
        NewBird();
    }

    public void NewBird() {
        BirdGroup bird = Instantiate(birdPrefab, spawn.position, Quaternion.identity).GetComponent<BirdGroup>();
        bird.Initialize(spawn.position, goal.position, cam);
        bird.OnReachGoal += NewBird;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherShip : MonoBehaviour
{
    public float food;
    public int harvestDistance;

    public int reach = 2;

    private int actionPoints = 2;
    private int remainingActionPoints;
    

    private Movable movable;
    public  HexagonsOutline outline;

    public int ActionPoints
    {
        get => actionPoints;
    }

    public int RemainingActionPoints
    {
        get => remainingActionPoints;
    }

    private void Start() {
        movable = GetComponent<Movable>();
        movable.OnReachEndTile += EndMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount == 1) {
            EndMove();
        }
    }

    public void BeginMove() {
        outline.Clear();
    }

    void EndMove() {
        List<TileProperties> reachables = movable.CurrentTile.InRange(harvestDistance);
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].IsInReachables = true;
        }
        outline.InitMesh(reachables);
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].IsInReachables = false;
        }
    }
}

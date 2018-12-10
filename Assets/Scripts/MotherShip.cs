using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movable))]
public class MotherShip : MonoBehaviour
{
    public float food;
    public int harvestDistance;

    public int reach = 2;

    public int actionPoints = 2;
    private int remainingActionPoints;
    

    private Movable movable;
    public  HexagonsOutline outline;

    public delegate void OnMotherShipDelegate();
    public OnMotherShipDelegate OnTurnBegin;
    public OnMotherShipDelegate OnEndMoving;

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
        remainingActionPoints = actionPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount == 1) {
            EndMove();
        }
    }

    public void BeginTurn() {
        remainingActionPoints = actionPoints;
        if (OnTurnBegin != null) {
            OnTurnBegin();
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
        
        remainingActionPoints -= movable.CurrentTile.ActionPointCost;
        if (OnEndMoving != null) {
            OnEndMoving();
        }
    }
}

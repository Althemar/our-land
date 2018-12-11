using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Movable), typeof(Inventory))]
public class MotherShip : MonoBehaviour
{
    

    public float food;
    public int harvestDistance;

    public int reach = 2;

    public int actionPoints = 2;
    private int remainingActionPoints;
    

    private Movable movable;
    public  HexagonsOutline outline;
    public Inventory inventory;

    public List<TileProperties> tileInRange;

    [SerializeField]
    public Resources resources;  

    public delegate void OnMotherShipDelegate();
    public OnMotherShipDelegate OnTurnBegin;
    public OnMotherShipDelegate OnEndMoving;
    public OnMotherShipDelegate OnRemainingPointsChanged;

    public int ActionPoints
    {
        get => actionPoints;
    }

    public int RemainingActionPoints
    {
        get => remainingActionPoints;
        set
        {
            remainingActionPoints = value;
            if (OnRemainingPointsChanged != null) {
                OnRemainingPointsChanged();
            }
        }
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
        tileInRange = movable.CurrentTile.InRange(harvestDistance);
        for (int i = 0; i < tileInRange.Count; i++) {
            tileInRange[i].IsInReachables = true;
        }
        outline.InitMesh(tileInRange);
        for (int i = 0; i < tileInRange.Count; i++) {
            tileInRange[i].IsInReachables = false;
        }
        
        remainingActionPoints -= movable.CurrentTile.ActionPointCost;
        if (OnEndMoving != null) {
            OnEndMoving();
        }
    }


    
}

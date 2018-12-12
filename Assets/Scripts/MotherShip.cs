using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Movable), typeof(Inventory))]
public class MotherShip : MonoBehaviour
{
    public int harvestDistance;

    public int reach = 2;

    public int actionPoints = 2;
    public HexagonsOutline outline;
     

    public ResourceType populationResource;

    public ResourceType foodResource;
    public float foodConsumption;

    

    private int remainingActionPoints;
    
    private Movable movable;

    private Inventory inventory;

    private List<TileProperties> tilesInRange;

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
            OnRemainingPointsChanged?.Invoke();
        }
    }

    public List<TileProperties> TilesInRange
    {
        get => tilesInRange;
    }

    public Inventory Inventory
    {
        get => inventory;
    }

    public Movable Movable
    {
        get => movable;
    }

    private void Start() {
        movable = GetComponent<Movable>();
        inventory = GetComponent<Inventory>();
        movable.OnReachEndTile += EndMove;
        remainingActionPoints = actionPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.FrameCount == 0) {
            EndMove();
        }
    }

    public void BeginTurn() {
        inventory.AddItem(foodResource, -foodConsumption * inventory.resources[populationResource]);
        
        remainingActionPoints = actionPoints;
        OnTurnBegin?.Invoke();
        OnRemainingPointsChanged?.Invoke();
    }

    public void BeginMove() {
        outline.Clear();
    }

    void EndMove() {
        tilesInRange = movable.CurrentTile.InRange(harvestDistance);
        for (int i = 0; i < tilesInRange.Count; i++) {
            tilesInRange[i].IsInReachables = true;
        }
        outline.InitMesh(tilesInRange);
        for (int i = 0; i < tilesInRange.Count; i++) {
            tilesInRange[i].IsInReachables = false;
        }
        
        remainingActionPoints -= movable.CurrentTile.ActionPointCost;
        OnEndMoving?.Invoke();
        OnRemainingPointsChanged?.Invoke();
    }
}

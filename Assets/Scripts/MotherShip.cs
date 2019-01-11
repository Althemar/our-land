using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(Movable), typeof(Inventory))]
public class MotherShip : MonoBehaviour
{
    public int harvestDistance;

    public int reach = 2;

    public int actionPoints = 2;
    public HexagonsOutline outline;
    public SkeletonAnimation spineShip;


    public ResourceType populationResource;

    public ResourceType foodResource;
    public float foodConsumption;

   
    private int remainingActionPoints;
    private bool onMove;

    private Movable movable;
    private Inventory inventory;

    private List<TileProperties> tilesInRange;

    [SerializeField]
    public Resources resources;  

    public delegate void OnMotherShipDelegate();
    public OnMotherShipDelegate OnTurnBegin;
    public OnMotherShipDelegate OnBeginMoving;
    public OnMotherShipDelegate OnEndMoving;
    public OnMotherShipDelegate OnRemainingPointsChanged;

    public int ActionPoints
    {
        get => actionPoints;
    }

    public bool OnMove {
        get => onMove;
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
        OnRemainingPointsChanged?.Invoke();
        Console.AddCommand("addActionPoints", CmdAddPA, "Add action points");
        Console.AddCommand("setMaxActions", CmdMaxPA, "Set the max of action points");
    }

    private void CmdAddPA(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            remainingActionPoints += n;
            OnRemainingPointsChanged?.Invoke();
        }
        else {
            Console.Write("Usage: addActionPoints [n] \nAdd n action points.");
        }
    }

    private void CmdMaxPA(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            actionPoints = n;
            OnRemainingPointsChanged?.Invoke();
        }
        else {
            Console.Write("Usage: setMaxActions [n] \nSet the max action points to n.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.FrameCount == 0) {
            ShowHarvestOutline();
        }
    }

    public void BeginTurn() {
        inventory.AddItem(foodResource, -foodConsumption * inventory.resources[populationResource]);
        
        remainingActionPoints = actionPoints;
        OnTurnBegin?.Invoke();
        OnRemainingPointsChanged?.Invoke();
    }

    public void ClearHarvestOutline() {
        outline.Clear();
    }

    public void ShowHarvestOutline() {
        tilesInRange = movable.CurrentTile.InRange(harvestDistance);
        for (int i = 0; i < tilesInRange.Count; i++) {
            tilesInRange[i].IsInReachables = true;
        }
        outline.InitMesh(tilesInRange);
        for (int i = 0; i < tilesInRange.Count; i++) {
            tilesInRange[i].IsInReachables = false;
        }
    }
    TileProperties goal;
    public void BeginMove(TileProperties goal) {
        Playtest.TimedLog("Player Move");

        onMove = true;
        OnBeginMoving?.Invoke();
        outline.Clear();
        spineShip.state.ClearTrack(0);
        spineShip.state.SetAnimation(0, "Decollage", false);
        spineShip.timeScale = 1;
        this.goal = goal;
        spineShip.state.Complete += ShipTakeOff;
    }

    private void ShipTakeOff(TrackEntry trackEntry) {
        Debug.Log("Move");
        movable.MoveToTile(goal);
    }

    private void DebugLog(TrackEntry trackEntry) {
        Debug.Log("Debug");
    }

    void EndMove() {
        ShowHarvestOutline();
        
        OnEndMoving?.Invoke();
        remainingActionPoints -= movable.CurrentTile.ActionPointCost;
        OnRemainingPointsChanged?.Invoke();
        spineShip.state.Complete -= ShipTakeOff;
        spineShip.timeScale = -2;
        onMove = false;
    }
}

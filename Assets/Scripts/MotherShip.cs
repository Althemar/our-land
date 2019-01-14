using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(Movable), typeof(Inventory))]
public class MotherShip : Updatable
{
    public int harvestDistance;
    
    
    public HexagonsOutline outline;
    public SkeletonAnimation spineShip;


    public ResourceType populationResource;

    [BoxGroup("Food")]
    public ResourceType foodResource;
    [BoxGroup("Food")]
    public float foodConsumption;

    [BoxGroup("Movement")]
    public ResourceType fuelResource;
    [BoxGroup("Movement")]
    public float movementBaseCost;
    [BoxGroup("Movement")]
    public float movementDistanceMultiplicator;

    [HideInInspector]
    public int remainingPopulationPoints;
    public int maxPopulationPoints;

    [HideInInspector]
    public List<ActivePopulationPoint> populationPoints;
    private List<ActivePopulationPoint> savedPopulationPoints;
 
    private bool onMove;

    private Movable movable;
    private Inventory inventory;
    private ReachableTilesDisplay reachableTilesDisplay;

    [HideInInspector]
    public TileProperties targetTile;

    private List<TileProperties> tilesInRange;

    [SerializeField]
    public Resources resources;

    

    public delegate void OnMotherShipDelegate();
    public OnMotherShipDelegate OnTurnBegin;
    public OnMotherShipDelegate OnBeginMoving;
    public OnMotherShipDelegate OnEndMoving;
    public OnMotherShipDelegate OnRemainingPointsChanged;

    public bool OnMove {
        get => onMove;
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
        reachableTilesDisplay = GetComponent<ReachableTilesDisplay>();
        movable.OnReachEndTile += EndMove;
        OnRemainingPointsChanged?.Invoke();
        remainingPopulationPoints = maxPopulationPoints;
        Console.AddCommand("addActionPoints", CmdAddPA, "Add action points");
        Console.AddCommand("setMaxActions", CmdMaxPA, "Set the max of action points");
        populationPoints = new List<ActivePopulationPoint>();
        savedPopulationPoints = new List<ActivePopulationPoint>();
        AddToTurnManager();
    }

    private void CmdAddPA(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            remainingPopulationPoints += n;
            OnRemainingPointsChanged?.Invoke();
        }
        else {
            Console.Write("Usage: addActionPoints [n] \nAdd n action points.");
        }
    }

    public void ClearActiveActionPoints() {
        while (populationPoints.Count > 0) {
            savedPopulationPoints.Add(populationPoints[0]);
            populationPoints[0].RemovePopulationPoint();
        }
    }

    private void CmdMaxPA(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            remainingPopulationPoints = n - (maxPopulationPoints - remainingPopulationPoints);
            maxPopulationPoints = n;
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

    public void BeginMove() {
        Playtest.TimedLog("Player Move");

        onMove = true;
        OnBeginMoving?.Invoke();
        outline.Clear();
        spineShip.state.ClearTrack(0);
        spineShip.state.SetAnimation(0, "Decollage", false);
        spineShip.timeScale = 1;
        spineShip.state.Complete += ShipTakeOff;
    }

    private void ShipTakeOff(TrackEntry trackEntry) {
        movable.MoveToTile(targetTile);
    }

    void EndMove() {
        ShowHarvestOutline();
        OnEndMoving?.Invoke();
        OnRemainingPointsChanged?.Invoke();
        spineShip.state.Complete -= ShipTakeOff;
        spineShip.timeScale = -2;
        onMove = false;
        targetTile = null;
        EndTurn();
    }

    public void CancelMove() {
        reachableTilesDisplay.UndisplayReachables();
        ShowHarvestOutline();
        targetTile = null;
        foreach (ActivePopulationPoint populationPoint in savedPopulationPoints) {
            populationPoints.Add(populationPoint);
            populationPoint.ReplacePopulationPoint();
        }
        savedPopulationPoints.Clear();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        if (targetTile != null) {
            Playtest.TimedLog("Player Move");
            OnBeginMoving?.Invoke();
            reachableTilesDisplay.UndisplayReachables();
            outline.Clear();
            BeginMove();
            inventory.AddItem(fuelResource, Mathf.Floor(-targetTile.ActionPointCost));
        } else {
            EndTurn();
        }
    }
}

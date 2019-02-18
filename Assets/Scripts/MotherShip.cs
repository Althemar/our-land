using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(Movable), typeof(Inventory))]
public class MotherShip : Updatable {
    public int harvestDistance;
    
    public HexagonsOutline outline;
    public SkeletonAnimation spineShip;
    private SortingGroup sorting;
    
    [BoxGroup("Food")]
    public ResourceType foodResource;
    [BoxGroup("Food")]
    public int foodConsumption;

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
    [HideInInspector]
    public List<ActivePopulationPoint> savedPopulationPoints;
 
    private bool onMove;

    private Movable movable;
    private Inventory inventory;
    private ReachableTilesDisplay reachableTilesDisplay;

    [HideInInspector]
    public TileProperties targetTile;

    private List<TileProperties> tilesInRange;

    [SerializeField]
    public Resources resources;

    [HideInInspector]
    public List<Bonus> bonuses = new List<Bonus>();
    public enum ActionType { Harvest, Move, FoodConsumption, Bonus, QuestReward }

    public delegate void OnMotherShipDelegate();
    public OnMotherShipDelegate OnTurnBegin;
    public OnMotherShipDelegate OnBeginMoving;
    public OnMotherShipDelegate OnEndMoving;
    public OnMotherShipDelegate OnRemainingPointsChanged;

    public delegate void InventoryChangeDelegate(ResourceType resource, int amount);
    public InventoryChangeDelegate OnResourceGained;

    public bool OnMove {
        get => onMove;
    }
    public List<TileProperties> TilesInRange {
        get => tilesInRange;
    }

    public Inventory Inventory {
        get => inventory;
    }

    public Movable Movable {
        get => movable;
    }

    public bool canFish = false;

    void Awake() {
        movable = GetComponent<Movable>();
        inventory = GetComponent<Inventory>();
        reachableTilesDisplay = GetComponent<ReachableTilesDisplay>();
        sorting = spineShip.GetComponent<SortingGroup>();
        movable.OnReachEndTile += EndMove;

        Console.AddCommand("addPopulationPoints", CmdAddPP, "Add temporary population points");
        Console.AddCommand("setPopulationPoints", CmdSetPP, "Set the number of population points");

        remainingPopulationPoints = maxPopulationPoints;
        populationPoints = new List<ActivePopulationPoint>();
        savedPopulationPoints = new List<ActivePopulationPoint>();
    }

    private void Start() {
        OnRemainingPointsChanged?.Invoke();
        AddToTurnManager();
        ShowHarvestOutline();
    }

    private void Update() {
        sorting.sortingOrder = -movable.CurrentTile.Position.y;
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    public void BeginTurn() {
        if (foodResource)
            AddItem(foodResource, -foodConsumption * maxPopulationPoints, ActionType.FoodConsumption);

        OnTurnBegin?.Invoke();
        OnRemainingPointsChanged?.Invoke();
    }

    public override void UpdateTurn() {
        base.UpdateTurn();

        foreach (Bonus b in bonuses) {
            b.BonusEffectEndTurn();
        }

        if (targetTile != null) {
            reachableTilesDisplay.UndisplayReachables();
            outline.Clear();
            BeginMove();
            AddItem(fuelResource, (int)Mathf.Floor(-targetTile.ActionPointCost), ActionType.Move);
            savedPopulationPoints.Clear();
        }
        else {
            EndTurn();
        }
    }

    public void MovementMode() {
        spineShip.state.Complete -= AfterGrounded;
        spineShip.state.Complete -= CanMove;
        canMove = false;
        //spineShip.state.ClearTrack(0);
        spineShip.state.SetAnimation(0, "Decollage_Herbe", false);
        AkSoundEngine.PostEvent("Play_TakeOff", this.gameObject);
        spineShip.state.Complete += CanMove;
    }

    public void HarvestMode() {
        spineShip.state.Complete -= AfterGrounded;
        spineShip.state.Complete -= CanMove;
        canMove = false;
        //spineShip.state.ClearTrack(0);
        spineShip.state.SetAnimation(0, "Atterissage_Herbe", false);
        AkSoundEngine.PostEvent("Play_Landing", this.gameObject);
    }

    bool canMove = false;
    void CanMove(TrackEntry trackEntry) {
        canMove = true;

        spineShip.state.SetAnimation(0, "Idle", true);

        if (onMove) {
            canMove = false;
            AkSoundEngine.PostEvent("Play_SFX_Ship_Move", this.gameObject);
            OnBeginMoving?.Invoke();
            outline.Clear();
            movable.MoveToTile(targetTile, false);
        }
    }

    public void BeginMove() {
        onMove = true;

        if(canMove) {
            canMove = false;
            AkSoundEngine.PostEvent("Play_SFX_Ship_Move", this.gameObject);
            OnBeginMoving?.Invoke();
            outline.Clear();
            movable.MoveToTile(targetTile, false);
        }
    }

    void EndMove() {
        ShowHarvestOutline();
        OnEndMoving?.Invoke();
        OnRemainingPointsChanged?.Invoke();

        spineShip.state.Complete -= CanMove;
        spineShip.state.ClearTrack(0);
        spineShip.state.SetAnimation(0, "Atterissage_Herbe", false);
        AkSoundEngine.PostEvent("Play_Landing", this.gameObject);
        spineShip.state.Complete += AfterGrounded;
    }


    private void AfterGrounded(TrackEntry trackEntry) {
        onMove = false;
        targetTile = null;
        EndTurn();
    }

    public void AddItem(ResourceType resource, int amount, ActionType action) {
        OnResourceGained?.Invoke(resource, amount);
        
        if(resource.name == "Population") {
            maxPopulationPoints += amount;
            remainingPopulationPoints += amount;
            return;
        }

        foreach (Bonus b in bonuses) {
            b.BonusEffectItem(action, resource, ref amount);
        }
        
        inventory.AddItem(resource, amount);
    }

    #region Harvest Outline
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

    public void ClearHarvestOutline() {
        outline.Clear();
    }
    #endregion

    #region Movement Cancel/Redo
    TileProperties savedTile;
    public void CancelMove() {
        reachableTilesDisplay.UndisplayReachables();
        savedTile = targetTile;
        targetTile = null;
    }

    public void RedoMove() {
        if (savedTile) {
            reachableTilesDisplay.InitReachableTiles(savedTile, movable);
            reachableTilesDisplay.RefreshPath(savedTile);
            reachableTilesDisplay.ValidReachables();
            targetTile = savedTile;
        }
    }
    #endregion

    #region Population Points
    public void ShowActivePopulationPoints() {
        foreach (ActivePopulationPoint populationPoint in savedPopulationPoints) {
            if (!populationPoint.IsValid() && remainingPopulationPoints >= 0)
                continue;
            populationPoints.Add(populationPoint);
            populationPoint.ReplacePopulationPoint();
        }
        savedPopulationPoints.Clear();
    }
    public void ClearActivePopulationPoints(bool save = true) {
        while (populationPoints.Count > 0) {
            if (save)
                savedPopulationPoints.Add(populationPoints[0]);
            populationPoints[0].RemovePopulationPoint();
            populationPoints.RemoveAt(0);
        }
    }
    #endregion

    #region Commands Population Points
    private void CmdAddPP(string[] args) {
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
            Console.Write("Usage: addPopulationPoints [n] \nAdd n temporary population points.");
        }
    }

    private void CmdSetPP(string[] args) {
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
            Console.Write("Usage: setPopulation [n] \nSet the number of population points to n.");
        }
    }
    #endregion
}

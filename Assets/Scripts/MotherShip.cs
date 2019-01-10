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
    
    public HexagonsOutline outline;
     

    public ResourceType populationResource;

    public ResourceType foodResource;
    public float foodConsumption;

   
    public int remainingPopulationPoints;

    [HideInInspector]
    public List<ActivePopulationPoint> populationPoints;
 
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
        OnRemainingPointsChanged?.Invoke();
        Console.AddCommand("addActionPoints", CmdAddPA, "Add action points");
        Console.AddCommand("setMaxActions", CmdMaxPA, "Set the max of action points");
        populationPoints = new List<ActivePopulationPoint>();
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

    private void CmdMaxPA(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            remainingPopulationPoints = n;
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
        OnBeginMoving?.Invoke();
        outline.Clear();
    }

    void EndMove() {
        ShowHarvestOutline();
        
        OnEndMoving?.Invoke();
        OnRemainingPointsChanged?.Invoke();
    }
        
}

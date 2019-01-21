using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    
    public enum TurnState
    {
        Player,
        PlayerUpdates,
        Entities, 
        Wind,
        Whirlwind,
        WindOrigin,
        Others
    }

    public static TurnManager Instance;

    public HexagonalGrid grid;
    public MotherShip motherShip;

    [ReorderableList]
    public List<TurnState> turnOrder;

    [ReorderableList]
    public List<TurnState> lateTurnOrder;

    [ReorderableList]
    public List<EntitySO> entitiesTypeOrder;


    private TurnState state = TurnState.Player;

    private Dictionary<EntitySO, List<Entity>> entitiesToUpdate;
    private List<Updatable> playersUpdates; 
    private List<Wind> windsToUpdate;
    private List<Whirlwind> whirlwindsToUpdate;
    private List<WindOrigin> windOriginsToUpdate;
    
    private int turnOrderIndex;
    private int entitiesTypeIndex;

    private int updatedObjects;
    private int nbObjectsToUpdate;

    private int turnCount = 1;

    public delegate void OnTurnDelegate();
    public event OnTurnDelegate OnEndTurn;

    private bool lateTurn;

    public TurnState State
    {
        get => state;
    }

    public int TurnCount
    {
        get => turnCount;
    }

    private void Awake() {
        if (!Instance) {
            Instance = this;
            entitiesToUpdate = new Dictionary<EntitySO, List<Entity>> ();
            for (int i = 0; i < entitiesTypeOrder.Count; i++) {
                entitiesToUpdate.Add(entitiesTypeOrder[i], new List<Entity>());
            }
            windsToUpdate = new List<Wind>();
            whirlwindsToUpdate = new List<Whirlwind>();
            windOriginsToUpdate = new List<WindOrigin>();
            playersUpdates = new List<Updatable>();
            Console.AddCommand("fastTurn", CmdFastTurn, "Fast Turn");
        }
        else {
            Destroy(gameObject);
        }
    }

    int fastTurn = 0;
    public bool isFastTurn = false;
    public float fastTurnSpeedMultiplicator = 2;
    private void CmdFastTurn(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid number");
                return;
            }

            fastTurn = n;
            TurnManager.Instance.OnEndTurn += UpdateFastTurn;
            isFastTurn = true;
            UpdateFastTurn();
        }
        else {
            Console.Write("Usage: fastTurn [n] \nDo n turn fast.");
        }
    }

    void UpdateFastTurn() {
        if (fastTurn > 0) {
            fastTurn--;
            TurnManager.Instance.EndTurn();
        }
        else {
            isFastTurn = false;
            TurnManager.Instance.OnEndTurn -= UpdateFastTurn;
        }
    }

    private void Update() {
        if (state != TurnState.Player && updatedObjects >= nbObjectsToUpdate) {
            if (state == TurnState.Entities) {
                entitiesTypeIndex++;
                if (entitiesTypeIndex == entitiesToUpdate.Count) {
                    NextTurnOrder();
                }
                else {
                    UpdateEntities();
                }
            }
            else if (state == TurnState.PlayerUpdates || state == TurnState.Wind || state == TurnState.Whirlwind || state == TurnState.WindOrigin) {
                NextTurnOrder();
            }
        }
    }

    private void NextTurnOrder() {
        turnOrderIndex++;
        if (turnOrderIndex == turnOrder.Count) {
            if (lateTurn)
                EndTurnUpdate();
            else
                LateTurnUpdate();
        }
        else {
            state = turnOrder[turnOrderIndex];
            ChooseObjectsToUpdate();
        }
    }

    public void AddToUpdate<T, T2>(T id, T2 obj) {
        if (id.GetType().BaseType == typeof(EntitySO)) {
            entitiesToUpdate[id as EntitySO].Add(obj as Entity);
        }
    }

    public void AddToUpdate<T>(T obj) {
        if (obj.GetType() == typeof(Wind)) {
            windsToUpdate.Add(obj as Wind);
        }
        if (obj.GetType() == typeof(Whirlwind)) {
            whirlwindsToUpdate.Add(obj as Whirlwind);
        }
        if (obj.GetType() == typeof(WindOrigin)) {
            windOriginsToUpdate.Add(obj as WindOrigin);
        }
        if (obj.GetType() == typeof(ActivePopulationPoint) || obj.GetType() == typeof(MotherShip)) {
            playersUpdates.Add(obj as Updatable);
        }
    }

    public void RemoveFromUpdate<T, T2>(T id, T2 obj) {
        if (id.GetType().BaseType == typeof(EntitySO)) {
            entitiesToUpdate[id as EntitySO].Remove(obj as Entity);
        }
    }

    public void RemoveFromUpdate<T>(T obj) {
        if (obj.GetType() == typeof(Wind)) {
            windsToUpdate.Remove(obj as Wind);
        }
        if (obj.GetType() == typeof(Whirlwind)) {
            whirlwindsToUpdate.Remove(obj as Whirlwind);
        }
        if (obj.GetType() == typeof(WindOrigin)) {
            windOriginsToUpdate.Remove(obj as WindOrigin);
        }
        if (obj.GetType() == typeof(ActivePopulationPoint) || obj.GetType() == typeof(MotherShip)) {
            playersUpdates.Remove(obj as Updatable);
        }
    }


    public void EndTurn() {
        if (state != TurnState.Player || motherShip.Movable.Moving || GameManager.Instance.GameState != GameState.Playing) {
            return;
        }
        Playtest.TimedLog("End turn " + TurnCount + " - PA " + motherShip.remainingPopulationPoints);
        lateTurn = false;
        state = turnOrder[0];
        turnOrderIndex = 0;
        ChooseObjectsToUpdate();
    }

    public void LateTurnUpdate() {
        state = lateTurnOrder[0];
        turnOrderIndex = 0;
        lateTurn = true;
        ChooseObjectsToUpdate();
    }

    public void EndTurnUpdate() {
        state = TurnState.Player;
        motherShip.BeginTurn();
        turnCount++;
        if (OnEndTurn != null) {
            OnEndTurn();
        }
    }

    private void ChooseObjectsToUpdate() {
        if (state == TurnState.Entities) {
            entitiesTypeIndex = 0;
            UpdateEntities();
        }
        else if (state == TurnState.Wind) {
            UpdateObjects(windsToUpdate);
        }
        else if (state == TurnState.Whirlwind) {
            UpdateObjects(whirlwindsToUpdate);
        }
        else if (state == TurnState.WindOrigin) {
            UpdateObjects(windOriginsToUpdate);
        }
        else if (state == TurnState.PlayerUpdates) {
            UpdateObjects(playersUpdates);
        }
    }

    public void UpdateEntities() {
        List<Entity> currentEntities = entitiesToUpdate[entitiesTypeOrder[entitiesTypeIndex]];

        UpdateObjects(currentEntities);
    }

    private void UpdateObjects<T>(List<T> toUpdate) where T : Updatable{
        updatedObjects = 0;
        for (int i = 0; i < toUpdate.Count; i++) {
            toUpdate[i].updated = false;
        }
        List<T> copy = new List<T>(toUpdate);
        nbObjectsToUpdate = 0;
        for (int i = 0; i < copy.Count; i++) {
            if (!copy[i].updated) {
                nbObjectsToUpdate++;
                if (!lateTurn)
                    copy[i].UpdateTurn();
                else
                    copy[i].LateUpdateTurn();
            }
        }
    }

    public void EntityUpdated() {
        updatedObjects++;
    }
}

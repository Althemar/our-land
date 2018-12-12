using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    
    public enum TurnState
    {
        Player,
        Entities, 
        Wind,
        Whirlwind,
        Others
    }

    public static TurnManager Instance;

    public HexagonalGrid grid;
    public MotherShip motherShip;

    [ReorderableList]
    public List<TurnState> turnOrder;

    [ReorderableList]
    public List<EntitySO> entitiesTypeOrder;


    private TurnState state = TurnState.Player;

    private Dictionary<EntitySO, List<Entity>> entitiesToUpdate;
    private List<Wind> windsToUpdate;
    private List<Whirlwind> whirlwindsToUpdate;
    
    private int turnOrderIndex;
    private int entitiesTypeIndex;

    private int updatedObjects;
    private int nbObjectsToUpdate;

    private int turnCount = 1;

    public delegate void OnTurnDelegate();
    public event OnTurnDelegate OnEndTurn;

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
        }
        else {
            Destroy(gameObject);
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
            else if (state == TurnState.Wind || state == TurnState.Whirlwind) {
                NextTurnOrder();
            }
        }
    }

    private void NextTurnOrder() {
        turnOrderIndex++;
        if (turnOrderIndex == turnOrder.Count) {
            EndTurnUpdate();
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
    }


    public void EndTurn() {
        if (state != TurnState.Player || motherShip.Movable.Moving) {
            return;
        }
        state = turnOrder[0];
        turnOrderIndex = 0;
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
                copy[i].UpdateTurn();
            }
        }
    }

    public void EntityUpdated() {
        updatedObjects++;
    }

}

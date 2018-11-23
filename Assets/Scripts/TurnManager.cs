using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    
    public enum TurnState
    {
        Player,
        Others
    }

    public static TurnManager Instance;

    public HexagonalGrid grid;

    public List<EntitySO> entitiesTypeOrder;


    private TurnState state = TurnState.Player;

    private Dictionary<EntitySO, List<Entity>> entitiesToUpdate;
    
    private int toUpdateIndex;

    private int updatedEntities;
    private int nbEntitiesToUpdate;

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

        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        
        if (state == TurnState.Others && updatedEntities == nbEntitiesToUpdate) {
            toUpdateIndex++;
            if (toUpdateIndex == entitiesToUpdate.Count) {
                state = TurnState.Player;
                turnCount++;
                if (OnEndTurn != null) {
                    OnEndTurn();
                }
            }
            else {
                updatedEntities = 0;
                UpdateEntities();
            }
        }
    }

    public void AddToUpdate<T, T2>(T id, T2 obj) {
        if (id.GetType() == typeof(EntitySO)) {
            entitiesToUpdate[id as EntitySO].Add(obj as Entity);
        }
    }

    public void RemoveFromUpdate<T, T2>(T id, T2 obj) {
        if (id.GetType().GetGenericTypeDefinition() == typeof(EntitySO)) {
            entitiesToUpdate[id as EntitySO].Remove(obj as Entity);
        }
    }

    public void EndTurn() {
        state = TurnState.Others;
        updatedEntities = 0;
        toUpdateIndex = 0;
        UpdateEntities();
    }

    public void UpdateEntities() {
        List<Entity> currentEntities = entitiesToUpdate[entitiesTypeOrder[toUpdateIndex]];
        for (int i = 0; i < currentEntities.Count; i++) {
            currentEntities[i].UpdateTurn();
        }
        nbEntitiesToUpdate = currentEntities.Count;
    }

    public void EntityUpdated() {
        updatedEntities++;
    }

}

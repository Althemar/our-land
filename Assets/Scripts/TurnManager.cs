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
    public TurnState state = TurnState.Player;

    public HexagonalGrid grid;

    private List<Entity> toUpdate;
    private int updatedEntities;

    private void Awake() {
        if (!Instance) {
            Instance = this;
            toUpdate = new List<Entity>();
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Update() {
        if (updatedEntities == toUpdate.Count) {
            state = TurnState.Player;
        }
    }

    public void AddToUpdate(Entity entity) {
        toUpdate.Add(entity);
    }

    public void EndTurn() {
        state = TurnState.Others;
        updatedEntities = 0;
        for (int i = 0; i < toUpdate.Count; i++) {
            toUpdate[i].UpdateEntity();
        }
    }

    public void EntityUpdated() {
        updatedEntities++;
    }

}

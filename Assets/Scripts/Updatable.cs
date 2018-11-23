using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Updatable : MonoBehaviour
{
    public abstract void UpdateTurn();

    public virtual void EndTurn() {
        TurnManager.Instance.EntityUpdated();
    }

    public abstract void AddToTurnManager();

    public abstract void RemoveFromTurnManager();

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Updatable : MonoBehaviour
{
    [HideInInspector]
    public bool updated = true;
    public bool updating = false;

    public delegate void OnEndTurnAction(Updatable up);
    public OnEndTurnAction OnEndTurn = delegate {};

    public virtual void UpdateTurn() {
        updating = true;
        updated = true;
    }

    public virtual void LateUpdateTurn() {
        updated = true;
        updating = true;
    }

    public virtual void EndTurn() {
        OnEndTurn(this);
        updating = false;
        TurnManager.Instance.EntityUpdated();
    }

    public abstract void AddToTurnManager();

    public abstract void RemoveFromTurnManager();

}

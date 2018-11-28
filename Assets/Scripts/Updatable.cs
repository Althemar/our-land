using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Updatable : MonoBehaviour
{
    [HideInInspector]
    public bool updated = true; 

    public virtual void UpdateTurn() {
        updated = true;
    }

    public virtual void EndTurn() {
        TurnManager.Instance.EntityUpdated();
    }

    public abstract void AddToTurnManager();

    public abstract void RemoveFromTurnManager();

}

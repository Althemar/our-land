using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Entity))]
public class StateController : MonoBehaviour {

    public State currentState;
    public State remainState;
    [HideInInspector] public Entity entity;

    private bool aiActive;


    void Awake () 
    {
        entity = GetComponent<Entity>();
    }

    public void SetupAI(bool aiActivation)
    {
        aiActive = aiActivation;
    }

    public void TurnUpdate()
    {
        if (!aiActive)
            return;
        currentState.UpdateState (this);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState) 
        {
            currentState = nextState;
            OnExitState ();
        }
    }

    private void OnExitState()
    {
    }
}
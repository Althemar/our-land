using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Entity))]
public class StateController : MonoBehaviour {

    public State currentState;
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
        if (! currentState.init) {
            currentState.InitState(this);
        }
        currentState.UpdateState (this);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != currentState) 
        {
            currentState.OnExitState(this);
            currentState = nextState;
            currentState.OnEnterState(this);
        }
    }
}
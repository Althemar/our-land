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
        currentState.InitState(this);
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

    public void LateTurnUpdate() {
        if (!aiActive)
            return;
        currentState.LateUpdateState(this);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != currentState) 
        {
            nextState.InitState(this);
            currentState.DeInitState(this);
            currentState.OnExitState(this);
            currentState = nextState;
            currentState.OnEnterState(this);
        }
    }
}
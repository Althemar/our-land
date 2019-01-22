using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class State : ScriptableObject 
{

    public Action[] actions;
    public Transition[] transitions;


    [ReorderableList]
    public Action[] lateActions;

    public bool init = false;

    private void Awake() {
        
    }

    public void InitState(StateController controller) {
        controller.entity.OnEndTurn += onEndTurn;
    }

    public void DeInitState(StateController controller) {
        controller.entity.OnEndTurn -= onEndTurn;
    }

    public void UpdateState(StateController controller)
    {
        DoActions (controller);
    }

    private void onEndTurn(Updatable up) {
        StateController st = up.GetComponent<StateController>();
        if (st != null) {
            CheckTransitions(st);
        } else {
            Debug.LogError("Wtf no state controller");
        }
    }

    public void LateUpdateState(StateController controller) {
        DoLateActions(controller);
    }

    private void DoActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++) {
            actions [i].Act (controller);
        }
    }

    private void DoLateActions(StateController controller) {
        for (int i = 0; i < lateActions.Length; i++) {
            lateActions[i].Act(controller);
        }
    }

    private void CheckTransitions(StateController controller)
    {
        for (int i = 0; i < transitions.Length; i++) 
        {
            bool decisionSucceeded = transitions [i].decision.Decide (controller);

            if (decisionSucceeded) {
                controller.TransitionToState (transitions [i].trueState);
            } else 
            {
                controller.TransitionToState (transitions [i].falseState);
            }
        }
    }

    public void OnEnterState(StateController controller) {
        for (int i = 0; i < actions.Length; i++) {
            actions [i].OnEnterState (controller);
        }
    }

    public void OnExitState(StateController controller) {
        for (int i = 0; i < actions.Length; i++) {
            actions [i].OnExitState (controller);
        }
    }

    private void OnDisable() {
        init = false;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class State : ScriptableObject 
{

    public Action[] actions;
    public Transition[] transitions;

    public bool init = false;

    private void Awake() {
        
    }

    public void InitState(StateController controller) {
        controller.entity.OnEndTurn += onEndTurn;
        init = true;
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

    private void DoActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++) {
            actions [i].Act (controller);
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

}
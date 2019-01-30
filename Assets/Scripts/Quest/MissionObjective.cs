using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class MissionObjective : MonoBehaviour
{
    public string description;
    public bool targetWithCamera;

    protected bool completed;

    public abstract bool Evaluate();

    public abstract void StartObjective();

}

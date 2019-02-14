﻿using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public string description;
    public bool targetWithCamera;
    public string progressText;

    protected bool completed;
    protected bool failed;

    public bool optional;

    public List<Reward> rewards;

    public bool Completed { get => completed; }
    public bool Failed { get => failed; }

    public delegate void Updated();
    public Updated OnUpdate;

    public virtual bool Evaluate() {
        OnUpdate?.Invoke();
        return false;
    }

    public abstract void StartObjective();

    public virtual string GetProgressText() {
        return progressText;
    }

}

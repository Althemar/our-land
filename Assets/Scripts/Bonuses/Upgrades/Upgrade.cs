using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    public string description;
    public int remainingResearchTurns;
    public bool alwaysActive = false;

    [HideInInspector]
    public bool unlocked;

    public abstract void DoUpgrade();
}
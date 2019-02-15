using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    [ReorderableList]
    public List<RuntimeAnimatorController> animators;
    public Animator animator;
    
    void Start() { 
        animator.runtimeAnimatorController = animators[Random.Range(0, animators.Count)];
    }
}

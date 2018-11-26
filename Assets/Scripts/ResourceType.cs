using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ResourceType : ScriptableObject {
    [SerializeField]
    public string name;
    [SerializeField]
    public Sprite icon;
}

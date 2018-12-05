using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public GameObject whirlwind;

    public List<Entity> blockingEntities;
    public List<CustomTile> blockingTiles;

    public static WindManager Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
}

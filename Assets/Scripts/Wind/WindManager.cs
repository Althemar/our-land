using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public GameObject whirlwind;
    public GameObject wind;


    [Header("Blocking")]
    public List<EntitySO> blockingEntities;
    public List<CustomTile> blockingTiles;

    [Header("Particle System")]
    public float normalRate;
    public float beginRate;

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

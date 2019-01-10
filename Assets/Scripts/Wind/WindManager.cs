using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public GameObject whirlwind;
    public GameObject wind;
    public Transform pools;

    SimplePool<Wind> windsPool;
    SimplePool<Whirlwind> whirlwindsPool;

    [Header("Blocking")]
    public List<EntitySO> blockingEntities;
    public List<CustomTile> blockingTiles;

    [Header("Particle System")]
    public float normalRate;
    public float beginRate;

    [HideInInspector]
    public int maxBaseDryness = 0;

    public List<WindOrigin> windOrigins;

    public static WindManager Instance;



    public SimplePool<Wind> WindsPool
    {
        get => windsPool;
    }

    public SimplePool<Whirlwind> WhirldwindsPool
    {
        get => whirlwindsPool;
    }

    private void Awake() {
        if (!Instance) {
            Instance = this;
            windsPool = SimplePoolHelper.PopulateSimplePool<Wind>(windsPool, wind, "Winds", 20, pools);
            whirlwindsPool = SimplePoolHelper.PopulateSimplePool<Whirlwind>(whirlwindsPool, whirlwind, "Whirlwinds", 3, pools);
            for (int i = 0; i < transform.childCount; i++) {
                WindOrigin wo = transform.GetChild(i).GetComponent<WindOrigin>();
                if (wo) {
                    windOrigins.Add(wo);
                }
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}

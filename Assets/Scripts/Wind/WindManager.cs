using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public GameObject whirlwind;
    public GameObject wind;

    SimplePool<Wind> windsPool;
    SimplePool<Whirlwind> whirlwindsPool;

    [Header("Blocking")]
    public List<EntitySO> blockingEntities;
    public List<CustomTile> blockingTiles;

    [Header("Particle System")]
    public float normalRate;
    public float beginRate;

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
            windsPool = PopulatePool<Wind>(windsPool, wind, "Winds", 20);
            whirlwindsPool = PopulatePool<Whirlwind>(whirlwindsPool, whirlwind, "Whirlwinds", 3);
        }
        else {
            Destroy(gameObject);
        }
    }

    private SimplePool<T> PopulatePool<T>(SimplePool<T> simplePool, GameObject blueprint, string name, int size) where T : Component {
        simplePool = SimplePoolHelper.GetPool<T>(name);
        simplePool.OnPush = (item) => item.gameObject.SetActive(false);
        simplePool.OnPop = (item) => item.gameObject.SetActive(true);

        simplePool.CreateFunction = (template) =>
        {
            T newObject = Instantiate(blueprint).GetComponent<T>();
            if (newObject) {
                newObject.transform.parent = transform;
            }

            return newObject;
        };
        simplePool.Populate(size);
        return simplePool;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : Entity
{
    [HideInInspector]
    public StaticEntitySO staticEntitySO;

    protected override void Start() {
        base.Start();
        GetComponent<SpriteRenderer>().sortingOrder = 10;
        staticEntitySO = entitySO as StaticEntitySO;
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Initialize();
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        EndTurn();
    }

    public override void Initialize(int population = -1) {
        base.Initialize(population);
        tile.staticEntity = this;
    }

    public override EntityType GetEntityType() {
        return EntityType.Static;
    }
}

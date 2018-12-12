using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : Entity
{
    [HideInInspector]
    public StaticEntitySO staticEntitySO;

    protected override void Start() {
        base.Start();
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        staticEntitySO = entitySO as StaticEntitySO;
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Initialize();
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        IncreasePopulation();
        TryCreateAnotherEntity(EntityType.Static);
        EndTurn();
    }

    public override void Initialize(float population = -1) {
        base.Initialize(population);
        tile.staticEntity = this;
    }
}

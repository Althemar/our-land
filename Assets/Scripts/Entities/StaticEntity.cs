using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : Entity
{
    [HideInInspector]
    public StaticEntitySO staticEntitySO;

    public int remainingTurnsBeforReproduction = -1;

    protected override void Start() {
        base.Start();
        GetComponent<SpriteRenderer>().sortingOrder = 10;
        staticEntitySO = entitySO as StaticEntitySO;
        remainingTurnsBeforReproduction = staticEntitySO.nbTurnsBeforeReproduction;
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

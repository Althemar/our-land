using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : Entity
{
    protected override void Start() {
        base.Start();
    }

    private void Update() {
        if (Time.frameCount == 1) {
            Initialize();
        }
    }

    public override void UpdateTurn() {
        IncreasePopulation();
        TryCreateAnotherEntity(EntityType.Static);
        EndTurn();
    }

    public override void Initialize() {
        base.Initialize();
        tile.staticEntity = this;
    }
}

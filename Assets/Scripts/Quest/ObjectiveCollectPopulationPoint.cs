using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollectPopulationPoint : Objective
{
    public GameObject populationPointPrefab;
    public ResourceType resource;
    public int goal;

    private TileProperties tile;

    private int count = 0;

    public override void StartObjective() {
        GameManager.Instance.motherShip.OnResourceGained += CollectResource;

        Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        tile = HexagonalGrid.Instance.GetTile(cellPosition);
        transform.position = tile.transform.position;
        if (tile.movingEntity) {
            tile.movingEntity.Kill();
        }
        MovingEntity populationPoint = Instantiate(populationPointPrefab, tile.transform.position, Quaternion.identity, transform).GetComponent<MovingEntity>();
        populationPoint.Initialize(1);
    }

    public void ResetCount()
    {
        count = 0;
    }

    private void CollectResource(ResourceType resource, int count) {
        if (this.resource == resource && this.count < goal) {
            this.count += count;
            if (this.count > goal) {
                this.count = goal;
            }
        }
    }

    public override bool Evaluate() {

        if (!completed && count >= goal) {
            completed = true;
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
        return "Collect " + count + " / " + goal + " " + resource.name;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollectPopulationPoint : Objective
{
    public MovingEntity populationPointPrefab;
    public ResourceType resource;
    public int goal;

    private TileProperties tile;

    private int count = 0;

    public bool woodLimit;
    public int maximumWood;

    private int usedWood = 0;


    public override void StartObjective() {
        GameManager.Instance.motherShip.OnResourceGained += CollectResource;

        Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        tile = HexagonalGrid.Instance.GetTile(cellPosition);
        transform.position = tile.transform.position;
        if (tile.movingEntity && tile.movingEntity.entitySO != populationPointPrefab.entitySO) {
            tile.movingEntity.Kill();
        }

        if (!optional) {
             MovingEntity populationPoint = Instantiate(populationPointPrefab, tile.transform.position, Quaternion.identity, transform).GetComponent<MovingEntity>();
            populationPoint.Initialize(1);
        }
       

        if (woodLimit) {
            GameManager.Instance.motherShip.OnBeginMoving += ConsumeWood;
        }
    }

    public void ConsumeWood() {
        usedWood += (int)Mathf.Floor(GameManager.Instance.motherShip.targetTile.ActionPointCost);
        if (!completed && usedWood > maximumWood) {
            failed = true;
        }
        if (usedWood >= maximumWood) {
            usedWood = maximumWood;
        }
        OnUpdate?.Invoke();
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

        if (!completed && woodLimit && usedWood > maximumWood) {
            failed = true;
        }

        if (!completed && count >= goal) {
            completed = true;
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
        if (!woodLimit) {
            return "Collect the population point";
        }
        else {
            return "Collect the point using less than " + (maximumWood - usedWood) + " wood";
        }
        
    }

    public override int Goal() {
        if (!optional) {
            return goal;
        }
        else {
            return maximumWood;
        }
    }

    public override int Progress() {
        if (!optional) {
            return count;
        }
        else {
            return usedWood;
        }
    }
}

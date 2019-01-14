using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActivePopulationPoint : Updatable
{
    public SpriteRenderer pointIsActiveSprite;
    public SpriteRenderer removePointSprite;

    private Entity entity;
    private int turnCount = 0;

    bool entityDestroyed;
    

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    public void InitPopulationPoint(Entity entity) {
        AddToTurnManager();
        Vector3 position = entity.Tile.transform.position;
        position.y -= 1;
        Entity otherEntity = null;
        if (entity.Tile.movingEntity == entity && entity.Tile.staticEntity != null) {
            otherEntity = entity.Tile.staticEntity;
        }
        else if (entity.Tile.staticEntity == entity && entity.Tile.movingEntity != null) {
            otherEntity = entity.Tile.movingEntity;
        }
        if (otherEntity && otherEntity.populationPoint) {
            otherEntity.populationPoint.transform.position += new Vector3(-0.5f, 0, 0);
            position.x += 0.5f;
        }
        transform.position = position;

        this.entity = entity;
        entity.populationPoint = this;
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        Debug.Log(entity.population);
        turnCount++;
        HarvestEntity();
        EndTurn();
        if(entity.population <= 0)
            RemovePopulationPoint();
    }

    private void HarvestEntity() {
        Debug.Log("Harvest");
        foreach (KeyValuePair<ResourceType, ArrayRessources> resource in entity.entitySO.resources) {
            float population;
            if (entity.population < 0) {
                population = 1;
            }
            else {
                population = Mathf.Floor(entity.population);
            }
            PopulationPoints.Instance.motherShip.Inventory.AddItem(resource.Key, resource.Value.gain[entity.HarvestedBonus]);
        }
        entity.Harvest();
        entityDestroyed = true;
        if (entity.entitySO)
            AkSoundEngine.PostEvent(entity.entitySO.harvestSound, GameManager.Instance.gameObject);
        DisplayHarvestedResources(entity); 
    }

    public void DisplayHarvestedResources(Entity entity) {
        StartCoroutine(DisplayHarvestedResourcesCoroutine(entity));
    }

    public IEnumerator DisplayHarvestedResourcesCoroutine(Entity entity) {
        ResourcesToHarvest resources = entity.entitySO.resources;
        Vector3 position = entity.transform.position;
        foreach (KeyValuePair<ResourceType, ArrayRessources> resource in resources) {
            ResourceHarvestedUI harvested = Instantiate(PopulationPoints.Instance.resourceGainedPrefab, position, Quaternion.identity, transform.parent).GetComponent<ResourceHarvestedUI>();
            harvested.Initialize(resource.Key, resource.Value.gain[entity.HarvestedBonus]);
            yield return new WaitForSeconds(1);
        }
    }

    public void RemovePopulationPoint() {
        RemoveFromTurnManager();
        PopulationPoints.Instance.RemovePopulationPoint(this);
        entity.populationPoint = null;
    }
}

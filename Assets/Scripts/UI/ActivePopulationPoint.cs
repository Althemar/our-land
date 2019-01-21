using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePopulationPoint : Updatable
{
    public Image pointIsActiveSprite;
    public Sprite normalSprite;

    private Entity entity;
    private Button button;
    private int turnCount = 0;

    bool entityDestroyed;

    public Vector3 beginPosition, targetPosition;


    private void Start() {
        button = GetComponent<Button>();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    

    public void InitPopulationPoint(Entity entity, int turnCount = 0) {
        button = GetComponent<Button>();

        button.transition = Selectable.Transition.SpriteSwap;
        //removePointSprite.gameObject.SetActive(false);

        AddToTurnManager();
        this.turnCount = turnCount;
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

        this.entity = entity;
        entity.populationPoint = this;
        beginPosition = PopulationPoints.Instance.motherShip.transform.position;
        targetPosition = position;
        StartCoroutine(MoveToTargetPosition());
    }

    public bool IsValid() {
        return entity != null;
    }

    public void ReplacePopulationPoint() {
        ActivePopulationPoint populationPoint = PopulationPoints.Instance.PopulationPointsPool.Pop(this);
        PopulationPoints.Instance.motherShip.remainingPopulationPoints--;
        PopulationPoints.Instance.motherShip.OnRemainingPointsChanged?.Invoke();
        InitPopulationPoint(entity);
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
        if (entity.entitySO && entity.entitySO.harvestSound != "")
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
        entity.populationPoint = null;
        beginPosition = transform.position;
        targetPosition = PopulationPoints.Instance.motherShip.transform.position;
        StartCoroutine(MoveToTargetPosition(true));

    }

    private IEnumerator MoveToTargetPosition(bool pushInPool = false) {
        if (pushInPool) {
            button.transition = Selectable.Transition.None;
        }
        pointIsActiveSprite.sprite = normalSprite;
        float progress = 0;
        while (progress <= 1) {
            transform.position = Vector3.Lerp(beginPosition, targetPosition, progress);
            progress += 0.07f;
            yield return null;
        }
        transform.position = Vector3.Lerp(beginPosition, targetPosition, 1);
        if (pushInPool) {
            PopulationPoints.Instance.RemovePopulationPoint(this);
        }
    }
}

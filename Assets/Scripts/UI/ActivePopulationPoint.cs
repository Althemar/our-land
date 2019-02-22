using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePopulationPoint : Updatable {
    public Image pointIsActiveSprite;
    public Sprite normalSprite;

    private Entity entity;
    private Button button;
    private int turnCount = 0;

    bool entityDestroyed;

    public Vector3 beginPosition, targetPosition;

    Coroutine moveCoroutine;


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
        if (entity.Tile.movingEntity == entity) {
            otherEntity = entity.Tile.staticEntity;
            if (otherEntity) {
                position.x += 1.1f;
            }
        }
        else if (entity.Tile.staticEntity == entity) {
            otherEntity = entity.Tile.movingEntity;
            if (otherEntity) {
                position.x -= 1.1f;
            }
        }

        this.entity = entity;
        entity.populationPoint = this;
        beginPosition = PopulationPoints.Instance.motherShip.transform.position;
        targetPosition = position;
        StartCoroutine(MoveToTargetPosition());
    }

    public bool IsValid() {
        return entity != null && entity.population > 0;
    }

    public void ReplacePopulationPoint() {

        /*
        if (!gameObject.activeSelf) {
            PopulationPoints.Instance.PopulationPointsPool.Push(this);
        }*/
        if (gameObject.activeSelf){
            StopCoroutine(moveCoroutine);
        }
        else {
            PopulationPoints.Instance.PopulationPointsPool.Pop(this);
        }

        PopulationPoints.Instance.motherShip.remainingPopulationPoints--;
        PopulationPoints.Instance.motherShip.OnRemainingPointsChanged?.Invoke();
        InitPopulationPoint(entity);
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        turnCount++;
        HarvestEntity();
        if (entity.population <= 0) {
            RemovePopulationPoint();
        }
        else {
            EndTurn();
        }
    }

    private void HarvestEntity() {
        foreach (KeyValuePair<ResourceType, ArrayRessources> resource in entity.entitySO.resources) {
            float population;
            if (entity.population < 0) {
                population = 1;
            }
            else {
                population = Mathf.Floor(entity.population);
            }

            int gain = resource.Value.gain[entity.HarvestedBonus];
            if (entity.entitySO.randomBonus.ContainsKey(resource.Key)){
                RandomBonus bonus = entity.entitySO.randomBonus[resource.Key].randomBonus[entity.HarvestedBonus];
                gain += Random.Range(bonus.minBonus, bonus.maxBonus + 1);
            }

            PopulationPoints.Instance.motherShip.AddItem(resource.Key, gain, MotherShip.ActionType.Harvest);
        }
        entity.Harvest();
        entityDestroyed = true;
        if (entity.entitySO && entity.entitySO.harvestSound != "")
            AkSoundEngine.PostEvent(entity.entitySO.harvestSound, GameManager.Instance.gameObject);
        DisplayHarvestedResources(entity);
    }

    public void DisplayHarvestedResources(Entity entity) {
        ResourcesToHarvest resources = entity.entitySO.resources;
        Vector3 position = entity.transform.position;
        float delay = 0f;
        foreach (KeyValuePair<ResourceType, ArrayRessources> resource in resources) {
            ResourceHarvestedUI harvested = Instantiate(PopulationPoints.Instance.resourceGainedPrefab, position, Quaternion.identity, transform.parent).GetComponent<ResourceHarvestedUI>();

            int gain = resource.Value.gain[entity.HarvestedBonus];
            if (entity.entitySO.randomBonus.ContainsKey(resource.Key)) {
                RandomBonus bonus = entity.entitySO.randomBonus[resource.Key].randomBonus[entity.HarvestedBonus];
                gain += Random.Range(bonus.minBonus, bonus.maxBonus + 1);
            }

            harvested.Initialize(resource.Key, gain, delay);
            delay += 0.8f;
        }
    }

    public void RemovePopulationPoint() {
        AkSoundEngine.PostEvent("Play_SFX_Button_PPOff", this.gameObject);
        RemoveFromTurnManager();
        entity.populationPoint = null;
        beginPosition = transform.position;
        targetPosition = PopulationPoints.Instance.motherShip.transform.position;
        moveCoroutine = StartCoroutine(MoveToTargetPosition(true));
    }

    private IEnumerator MoveToTargetPosition(bool pushInPool = false) {
        if (pushInPool) {
            button.transition = Selectable.Transition.None;
        }
        pointIsActiveSprite.sprite = normalSprite;
        float progress = 0;
        while (progress <= 1) {
            transform.position = Vector3.Lerp(beginPosition, targetPosition, progress);
            progress += 5f * Time.deltaTime;
            yield return null;
        }
        transform.position = Vector3.Lerp(beginPosition, targetPosition, 1);
        if (pushInPool) {
            PopulationPoints.Instance.RemovePopulationPoint(this);
        }
    }
}

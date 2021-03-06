﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesHarvestableUI : MonoBehaviour {
    public GameObject harvestEntityPrefab;
    public MotherShip motherShip;
    public PopulationPoints activePopulationPoints;

    private TileProperties currentTile;
    private bool displaying;

    int sizePool = 2;
    HarvestEntityUI[] pool;
    public InfoEntityUI info;

    int buttonsCount = 0;

    public bool Displaying {
        get => displaying;
    }

    public TileProperties CurrentTile {
        get => currentTile;
    }

    private void Awake() {
        pool = new HarvestEntityUI[sizePool];
        for (int i = 0; i < sizePool; i++) {
            GameObject item = Instantiate(harvestEntityPrefab, Vector3.zero, Quaternion.identity, transform);
            item.SetActive(false);
            pool[i] = item.GetComponent<HarvestEntityUI>();
        }
    }

    public void NewEntitiesToHarvest(TileProperties tile) {
        if (!tile) {
            return;
        }

        Vector3 position = tile.transform.position;
        position.y += 2;
        position.z = transform.position.z;

        buttonsCount = 0;
        if (tile.staticEntity && !tile.staticEntity.populationPoint && tile.movingEntity &&!tile.movingEntity.populationPoint) {
            Vector3 secondPosition = position;
            position.x -= 2.5f;
            secondPosition.x += 2.5f;
            InstantiateHarvestUI(position, tile.staticEntity);
            buttonsCount++;
            InstantiateHarvestUI(secondPosition, tile.movingEntity);
            displaying = true;
        }
        else if (tile.staticEntity && !tile.staticEntity.populationPoint) {
            InstantiateHarvestUI(position, tile.staticEntity);
            displaying = true;
        }
        else if (tile.movingEntity && !tile.movingEntity.populationPoint) {
            InstantiateHarvestUI(position, tile.movingEntity);
            displaying = true;
        }
        buttonsCount++;
    }

    public void ShowInfo(TileProperties tile) {
        if (!tile) {
            return;
        }
        
        if (tile.movingEntity) {
            info.Initialize(tile.movingEntity, this);
            info.gameObject.SetActive(true);
        }
        else if (tile.staticEntity) {
            info.Initialize(tile.staticEntity, this);
            info.gameObject.SetActive(true);
        }
        else {
            info.gameObject.SetActive(false);
        }
    }

    private void InstantiateHarvestUI(Vector3 position, Entity entity) {
        HarvestEntityUI harvestEntity = pool[buttonsCount];
        harvestEntity.transform.position = position;
        harvestEntity.gameObject.SetActive(true);
        harvestEntity.Initialize(entity, this);
    }

    public void Clear() {
        for (int i = 0; i < buttonsCount; i++) {
            pool[i].gameObject.SetActive(false);
        }
        displaying = false;
    }

    public bool CursorIsOnButton() {
        for (int i = 0; i < buttonsCount; i++) {
            if (pool[i].gameObject.activeSelf && pool[i].PointerIsOnButton) {
                return true;
            }
        }
        return false;
    }

    
}

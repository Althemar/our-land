﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesHarvestableUI : MonoBehaviour
{
    public GameObject harvestEntityPrefab;
    public MotherShip motherShip;
    
    private TileProperties currentTile;
    private bool displaying;

    int sizePool = 2;
    HarvestEntityUI[] pool;

    int buttonsCount = 0;

    public bool Displaying
    {
        get => displaying;
    }

    public TileProperties CurrentTile
    {
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
        if (tile.staticEntity && tile.movingEntity) {
            Vector3 secondPosition = position;
            position.x -= 1.25f;
            secondPosition.x += 1.25f;
            InstantiateHarvestUI(position, tile.staticEntity);
            buttonsCount++;
            InstantiateHarvestUI(secondPosition, tile.movingEntity);
            displaying = true;
        }
        else if (tile.staticEntity) {
            InstantiateHarvestUI(position, tile.staticEntity);
            displaying = true;
        }
        else if (tile.movingEntity) {
            InstantiateHarvestUI(position, tile.movingEntity);
            displaying = true;
        }
        buttonsCount++;
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
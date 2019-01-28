using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesHarvestableUI : MonoBehaviour {
    public GameObject harvestEntityPrefab;
    public MotherShip motherShip;
    public PopulationPoints activePopulationPoints;
    
    private bool displaying;
    
    public InfoEntityUI info;

    int buttonsCount = 0;

    List<HarvestEntityUI> instanciate = new List<HarvestEntityUI>();

    public bool Displaying {
        get => displaying;
    }

    void Start() {
        motherShip.OnRemainingPointsChanged += UpdateButtons;
    }

    void OnDestroy() {
        motherShip.OnRemainingPointsChanged += UpdateButtons;
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

    public void EntitiesToHarvest(List<TileProperties> tilesInRange) {
        foreach (TileProperties tile in tilesInRange) {
            Vector3 position = tile.transform.position;
            position.y -= 1;
            position.z = transform.position.z;

            if (tile.staticEntity && tile.movingEntity) {
                Vector3 movingPosition = position;
                position.x -= 0.5f;
                movingPosition.x += 0.5f;

                AddButton(tile.staticEntity, position);
                AddButton(tile.movingEntity, movingPosition);

                displaying = true;
            }
            else if (tile.staticEntity) {
                AddButton(tile.staticEntity, position);

                displaying = true;
            }
            else if (tile.movingEntity) {
                AddButton(tile.movingEntity, position);

                displaying = true;
            }
        }
    }

    public void UpdateButtons() {
        foreach (HarvestEntityUI button in instanciate) {
            button.Refresh();
        }
    }

    void AddButton(Entity entity, Vector3 position) {
        HarvestEntityUI button = Instantiate(harvestEntityPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<HarvestEntityUI>();
        button.transform.position = position;
        button.Initialize(entity, this);
        instanciate.Add(button);
    }

    public void Clear() {
        for (int i = 0; i < instanciate.Count; i++) {
            Destroy(instanciate[i].gameObject);
        }
        instanciate.Clear();
        displaying = false;
    }
    
}

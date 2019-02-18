using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesHarvestableUI : MonoBehaviour {
    public GameObject harvestEntityPrefab;
    public MotherShip motherShip;
    public PopulationPoints activePopulationPoints;
    
    private bool displaying;

    int buttonsCount = 0;

    List<PopulationSlot> instanciate = new List<PopulationSlot>();

    public bool Displaying {
        get => displaying;
    }

    void Start() {
        motherShip.OnRemainingPointsChanged += UpdateButtons;
        TurnManager.Instance.OnEndTurn += EntitiesToHarvest;
        EntitiesToHarvest();
    }

    void OnDestroy() {
        motherShip.OnRemainingPointsChanged += UpdateButtons;
    }

    public void ShowInfo(TileProperties tile) {
        if (!tile) {
            return;
        }
    }

    public void EntitiesToHarvest() {
        Clear();
        foreach (TileProperties tile in motherShip.TilesInRange) {
            Vector3 position = tile.transform.position;
            position.y -= 1;
            position.z = transform.position.z;

            if (tile.staticEntity && tile.movingEntity) {
                Vector3 movingPosition = position;
                position.x -= 0.5f;
                movingPosition.x += 0.5f;

                AddButton(tile.staticEntity, position);
                AddButton(tile.movingEntity, movingPosition);

            }
            else if (tile.staticEntity) {
                AddButton(tile.staticEntity, position);
            }
            else if (tile.movingEntity) {
                AddButton(tile.movingEntity, position);
            }
        }
    }

    public void UpdateButtons() {
        foreach (PopulationSlot button in instanciate) {
            button.Refresh();
        }
    }

    public void AddButton(Entity entity, Vector3 position) {
        if (entity.population <= 0 || (entity.entitySO == GameManager.Instance.fishPrefab.entitySO && !motherShip.canFish)) {
            return;
        }
        PopulationSlot button = Instantiate(harvestEntityPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<PopulationSlot>();
        button.transform.position = position;
        button.Initialize(entity, this);
        instanciate.Add(button);
        displaying = true;

    }

    public void Clear() {
        for (int i = 0; i < instanciate.Count; i++) {
            Destroy(instanciate[i].gameObject);
        }
        instanciate.Clear();
        displaying = false;
    }
    
}

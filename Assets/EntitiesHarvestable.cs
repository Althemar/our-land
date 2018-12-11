using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesHarvestable : MonoBehaviour
{
    public GameObject harvestEntityPrefab;
    public MotherShip motherShip;

    private List<HarvestEntityUI> harvestEntityUI;
    private TileProperties currentTile;
    private bool displaying;



    public bool Displaying
    {
        get => displaying;
    }

    public TileProperties CurrentTile
    {
        get => currentTile;
    }


    // Start is called before the first frame update
    void Start()
    {
        harvestEntityUI = new List<HarvestEntityUI>();
    }

    public void NewEntitiesToHarvest(TileProperties tile) {
        if (!tile) {
            return;
        }

        Vector3 position = tile.transform.position;
        position.y += 2;
        position.z = transform.position.z;
        if (tile.staticEntity && tile.movingEntity) {
            Vector3 secondPosition = position;
            position.x -= 1.25f;
            secondPosition.x += 1.25f;
            InstantiateHarvestUI(position, tile.staticEntity);
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
    }

    private void InstantiateHarvestUI(Vector3 position, Entity entity) {
        HarvestEntityUI harvestEntity = Instantiate(harvestEntityPrefab, position, Quaternion.identity, transform).GetComponent<HarvestEntityUI>();
        harvestEntityUI.Add(harvestEntity);
        harvestEntity.Initialize(entity, this);
    }

    public void Clear() {
        for (int i = 0; i < harvestEntityUI.Count; i++) {
            Destroy(harvestEntityUI[i].gameObject);
        }
        harvestEntityUI.Clear();
        displaying = false;
    }

    public bool CursorIsOnButton() {
        for (int i = 0; i < harvestEntityUI.Count; i++) {
            if (harvestEntityUI[i].PointerIsOnButton) {
                return true;
            }
        }
        return false;
    }
}

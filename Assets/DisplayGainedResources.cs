using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGainedResources : MonoBehaviour
{

    public void Display(Entity entity) {
        StartCoroutine(DisplayHarvestedResourcesCoroutine(entity));

    }

    public IEnumerator DisplayHarvestedResourcesCoroutine(Entity entity) {
        ResourcesToHarvest resources = entity.entitySO.resources;
        Vector3 position = entity.transform.position;
        foreach (KeyValuePair<ResourceType, ArrayRessources> resource in resources) {
            ResourceHarvestedUI harvested = Instantiate(PopulationPoints.Instance.resourceGainedPrefab, position, Quaternion.identity, transform.parent).GetComponent<ResourceHarvestedUI>();

            int gain = resource.Value.gain[entity.HarvestedBonus];
            if (entity.entitySO.randomBonus.ContainsKey(resource.Key)) {
                RandomBonus bonus = entity.entitySO.randomBonus[resource.Key].randomBonus[entity.HarvestedBonus];
                gain += Random.Range(bonus.minBonus, bonus.maxBonus + 1);
            }

            harvested.Initialize(resource.Key, gain);
            yield return new WaitForSeconds(1);
        }
    }
}

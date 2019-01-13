using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestFeedbackUI : MonoBehaviour {

    void Start() {
        MovingEntity.OnHarvest += ShowHarvest;
    }

    void ShowHarvest(MovingEntity from, Entity target) {
        if (from.movingEntitySO.eatFeedback) {
            Vector3 position = (from.transform.position + target.transform.position) / 2f;
            KillFeedbackUI harvested = Instantiate(from.movingEntitySO.eatFeedback, position, Quaternion.identity, this.transform).GetComponent<KillFeedbackUI>();
            harvested.Initialize();
        }
    }

}

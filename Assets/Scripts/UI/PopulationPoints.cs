using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationPoints : MonoBehaviour {
    public MotherShip motherShip;
    public GameObject populationPointPrefab;
    public GameObject resourceGainedPrefab;
    private SimplePool<ActivePopulationPoint> populationPointsPool;

    public static PopulationPoints Instance;

    public SimplePool<ActivePopulationPoint> PopulationPointsPool {
        get => populationPointsPool;
    }

    private void Start() {
        if (!Instance) {
            populationPointsPool = SimplePoolHelper.PopulateSimplePool(populationPointsPool, populationPointPrefab, "activePopulationPoints", motherShip.remainingPopulationPoints, transform);
            Instance = this;
        }
    }

    public void PlacePopulationPoint(Entity entity) {
        if (entity.populationPoint != null)
            return;
        ActivePopulationPoint populationPoint = populationPointsPool.Pop();
        populationPoint.InitPopulationPoint(entity);
        motherShip.populationPoints.Add(populationPoint);
        motherShip.remainingPopulationPoints--;
        motherShip.OnRemainingPointsChanged?.Invoke();
        if (motherShip.targetTile) {
            motherShip.GetComponent<ReachableTilesDisplay>().UndisplayReachables();
            motherShip.targetTile = null;
        }
    }

    public void RemovePopulationPoint(ActivePopulationPoint point) {
        PopulationPointsPool.Push(point);
        motherShip.populationPoints.Remove(point);
        motherShip.remainingPopulationPoints++;
        motherShip.OnRemainingPointsChanged?.Invoke();
    }
}

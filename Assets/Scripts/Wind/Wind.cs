using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Wind : MonoBehaviour
{
    public Wind previous;
    public List<Wind> next;
    public HexDirection direction;

    public WindOrigin windOrigin;

    public TileProperties tile;


    private bool previousAlreadyUpdated;

    public ParticleSystem ps;
    private List<Vector4> custom1;

    private void Awake() {
        next = new List<Wind>();
        custom1 = new List<Vector4>();
    }

    private void Update() {
        if (!gameObject.activeSelf) {
            return;
        }

        if (ps.isPlaying ) {
            Particle[] particles = new Particle[ps.particleCount];
            ps.GetParticles(particles);
            ps.GetCustomParticleData(custom1, ParticleSystemCustomData.Custom1);
            for (int i = 0; i < particles.Length; i++) {

                TileProperties tile = HexagonalGrid.Instance.GetTile(HexagonalGrid.Instance.Tilemap.WorldToCell(particles[i].position));

                if (tile && tile.wind) {

                    if (tile.wind.next.Count == 2) {
                        Vector3 goal = new Vector3(custom1[i].x, custom1[i].y, custom1[i].z);
                        bool goalIsSet = false;
                        for (int j = 0; j < tile.wind.next.Count; j++) {
                            Vector3 nextDir = HexagonalGrid.Instance.Metrics.GetBorder(tile.wind.next[j].direction);
                            if (nextDir == goal) {
                                goalIsSet = true;
                                break;
                            }
                        }

                        if (!goalIsSet) {
                            Vector3 dir = HexagonalGrid.Instance.Metrics.GetBorder((int)tile.wind.next[Random.Range(0, tile.wind.next.Count)].direction);
                            custom1[i] = new Vector4(dir.x, dir.y, dir.z, 0);

                        }
                        else {
                            particles[i].velocity = Vector3.Lerp(particles[i].position, goal, custom1[i].w);
                            Vector4 custom = custom1[i];
                            custom.w += 0.02f;
                            custom1[i] = custom;
                        }

                    }
                    else if (tile.wind.next.Count == 1) {
                        particles[i].velocity = HexagonalGrid.Instance.Metrics.GetBorder(tile.wind.next[0].direction);
                    }
                    else {
                        particles[i].velocity = HexagonalGrid.Instance.Metrics.GetBorder(tile.wind.direction);
                    }
                }
                else {
                    particles[i].remainingLifetime = Mathf.Min(1.0f, particles[i].remainingLifetime);
                    /*if (particles[i].remainingLifetime > 1) {
                        particles[i].remainingLifetime = Random.Range(0, 0.3f) ;
                    }*/
                }
            }
            ps.SetParticles(particles);
            ps.SetCustomParticleData(custom1, ParticleSystemCustomData.Custom1);
        } 
    }

    public void InitializeChildWind(TileProperties tile, Wind previous, HexDirection direction) {
        transform.position = tile.transform.position;
        this.previous = previous;
        if (previous) {
            previous.next.Add(this);
        }
        this.tile = tile;
        this.direction = direction;

        next.Clear();
        custom1.Clear();
        tile.wind = this;
        

        ps = GetComponentInChildren<ParticleSystem>();
        EmissionModule emission = ps.emission;
        if (previous == null) {
            emission.rateOverTime = WindManager.Instance.beginRate;
        }
        else {
            emission.rateOverTime = WindManager.Instance.normalRate;
        }
        ps.Play();   
    }

    public void DestroyWind(bool destroyNextWinds = false) {
        if (previous) {
            previous.next.Remove(this);
        }
        for (int i = 0; i < next.Count; i++) {
            next[i].previous = null;
        }
        tile.Tilemap.SetColor(tile.Position, Color.white);
        tile.wind = null;
        windOrigin = null;

        if (!ps.isPlaying) {
            WindManager.Instance.WindsPool.Push(this);
        }
        else {
            StartCoroutine(WaitBeforeDestroy());
        }

        if (destroyNextWinds) {
            foreach (Wind nextWind in next) {
                nextWind.DestroyWind(true);
            }
        }
    }

    private bool TryCreateNewWind(HexDirection nextDirection) {
        TileProperties nextTile = tile.GetNeighbor(nextDirection);
        if (CanCreateWindOnTile(nextTile)) {

            Wind newWind = WindManager.Instance.WindsPool.Pop();
            newWind.transform.position = nextTile.transform.position;
            
            next.Add(newWind);
            newWind.InitializeChildWind(nextTile, this, nextDirection);
            return true;
        }
        return false;
    }

    public bool CanCreateWindOnTile(TileProperties nextTile) {
        if (!nextTile || nextTile.wind) {
            return false;
        }
        if ((nextTile.staticEntity && WindManager.Instance.blockingEntities.Contains(nextTile.staticEntity.staticEntitySO)) 
            || (nextTile.movingEntity && WindManager.Instance.blockingEntities.Contains(nextTile.movingEntity.movingEntitySO))) {
            return false;
        }
        if (WindManager.Instance.blockingTiles.Contains(nextTile.Tile)) {
            return false;
        }
        return true;
    }

    public IEnumerator WaitBeforeDestroy() {
        ps.Stop();
        float timeToWait = ps.main.startLifetime.constant;
        foreach (Wind wind in next) {
            EmissionModule emission = wind.ps.emission;
            emission.rateOverTime = WindManager.Instance.beginRate;
        }
        yield return new WaitForSeconds(timeToWait);

        WindManager.Instance.WindsPool.Push(this);
    }
}

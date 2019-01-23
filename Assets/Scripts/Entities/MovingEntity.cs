using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[RequireComponent(typeof(Movable))]
public class MovingEntity : Entity
{
    [HideInInspector]
    public Movable movable;

    [HideInInspector]
    public MovingEntitySO movingEntitySO;
    
    public GameObject[] NWTarget;
    public GameObject[] WTarget;
    public GameObject[] SWTarget;
    public GameObject[] CenterTarget;
    public SkeletonAnimation[] SpineTarget;

    public SkeletonDataAsset NWSkele;
    public SkeletonDataAsset WSkele;
    public SkeletonDataAsset SWSkele;


    private EntityHungerState hunger;

    private Entity target;
    private bool stopBefore;
    private bool isMoving = false;
    [HideInInspector]
    public bool hasFled = false;

    private int baseSorting;

    [HideInInspector]
    public TileProperties previewTile;

    public delegate void OnHarvestDelegate(MovingEntity from, Entity target);
    public static OnHarvestDelegate OnHarvest;
    
    public bool isHungry = false; //used by actions
    public int remainingTurnsBeforeHungry = -1;
    public int remainingTurnsBeforeDie = -1;

    [HideInInspector]
    public bool harvestAnimation;

    protected override void Start() {
        base.Start();
        movable = GetComponent<Movable>();
        movable.hexGrid = TurnManager.Instance.grid;
        movable.OnReachEndTile += EndMoving;
        movable.OnChangeDirection += UpdateSprite;
        OnPopulationChange += UpdateSprite;
        if (tile) {
            movable.CurrentTile = tile;
            tile.movable = movable;
        }

        movingEntitySO = entitySO as MovingEntitySO;
        hunger = EntityHungerState.Full;

    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Initialize();
        }
    }

    HexDirection currentDir;
    public void UpdateSprite() {
        UpdateSprite(currentDir);
    }

    public void UpdateSprite(HexDirection dir, bool noDir = false) {
        if (this == null) {
            return;
        }
        currentDir = dir;

        if (!noDir) {

        }
        float flip = 1;
        if(dir == HexDirection.NE || dir == HexDirection.E || dir == HexDirection.SE)
            flip = -1;
        if(dir == HexDirection.NW || dir == HexDirection.NE)
            flip *= -1;

        activatedSkeletons.Clear();

        for (int i = 0; i < SpineTarget.Length; i++) {
            bool activate = population > i;
            SpineTarget[i].gameObject.SetActive(activate);
            if (activate) {
                activatedSkeletons.Add(SpineTarget[i]);
            }
        }

        StopAllCoroutines();

        GameObject[] target = null;
        for(int i = 0; i < 3; i++) {
            if (noDir) {
                dir = (HexDirection)Random.Range(0, 6);
            }

            int index = (!noDir) ? i : Random.Range(0, CenterTarget.Length / 3) + (i * (CenterTarget.Length / 3));

            switch (dir) {
                case HexDirection.NW:
                case HexDirection.NE:
                    SpineTarget[i].skeletonDataAsset = NWSkele;
                    target = (!noDir) ? NWTarget : CenterTarget;
                    StartCoroutine(GoToTarget(SpineTarget[i].gameObject, target[index], -flip));
                    break;
                case HexDirection.W:
                case HexDirection.E:
                    SpineTarget[i].skeletonDataAsset = WSkele;
                    target = (!noDir) ? WTarget : CenterTarget;
                    StartCoroutine(GoToTarget(SpineTarget[i].gameObject, target[index], flip));
                    break;
                case HexDirection.SW:
                case HexDirection.SE:
                    SpineTarget[i].skeletonDataAsset = SWSkele;
                    target = (!noDir) ? SWTarget : CenterTarget;
                    StartCoroutine(GoToTarget(SpineTarget[i].gameObject, target[index], flip));
                    break;
            }
            SpineTarget[i].initialFlipX = (flip == -1);
            SpineTarget[i].Initialize(true);
        }
        
    }

    IEnumerator GoToTarget(GameObject entity, GameObject target, float flip) {
        Vector3 pos = target.transform.localPosition;
        pos.x *= flip;
        pos = target.transform.localToWorldMatrix * pos;
        ChangeAnimation("Walk", true);
        while (Vector3.Distance(entity.transform.position, target.transform.parent.position + pos) > 0.01f) {
            entity.transform.position = Vector3.MoveTowards(entity.transform.position, target.transform.parent.position + pos, Time.deltaTime);
            yield return null;
        }
        ChangeAnimation("Idle", true);
    }

    public void UpdateSkeletons(GameObject container, GameObject[] sprites, float flip, HexDirection dir, HexDirection dir1, HexDirection dir2) {
        container.transform.localScale = new Vector3(flip, 1, 1);
        for (int i = 0; i < sprites.Length; i++) {
            bool activate = (dir == dir1 || dir == dir2) && population > i;
            sprites[i].SetActive(activate);
            if (activate) {
                activatedSkeletons.Add(sprites[i].GetComponent<SkeletonAnimation>());
            }
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();

        if (!isMoving) EndTurn();
    }

    private Movable.OnMovableDelegate eventAfterMove;
    public Stack<TileProperties> MoveTo(TileProperties to, Movable.OnMovableDelegate onEndMove, bool preview = false) {
        var pathToTarget = AStarSearch.Path(tile, to, entitySO.availableTiles, null, preview);
        if (pathToTarget != null && pathToTarget.Count >= 0 && !preview) {
            TileProperties newTile = movable.MoveToward(pathToTarget, movingEntitySO.movementPoints, to.movingEntity != null);
            if(newTile) {
                tile.movable = null;
                tile.movingEntity = null;
                eventAfterMove = onEndMove;
                tile = newTile;
                tile.movingEntity = this;
                tile.movable = movable;

                isMoving = true;
                SetPreviewTile(newTile, false);
                ChangeAnimation("Walk", true);
            } else {
                onEndMove?.Invoke();
            }
        } else {
            onEndMove?.Invoke();
        }
        return pathToTarget;
    }

    public override EntityType GetEntityType() {
        return EntityType.Moving;
    }

    public void Harvest(Entity target) {
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++) {
            if (Tile.GetNeighbor(dir) == target.Tile) {
                UpdateSprite(dir);
                break;
            }
        }
        OnHarvest(this, target);

        target.Eaten(1);

        /*int remainingFood = population - reserve;
        if (target.population > remainingFood) { // if there is more than enough food        
            reserve += remainingFood;
            target.Eaten(remainingFood);
        } else {
            reserve += target.population;
            target.Eaten(target.population);
        }
        */
    }

    public override void Initialize(int population = -1) {
        base.Initialize(population);
        remainingTurnsBeforeHungry = movingEntitySO.nbTurnsToBeHungry;
        remainingTurnsBeforeDie = movingEntitySO.nbTurnsToDie;
        tile.movingEntity = this;
        tile.movablePreview = movable;
        previewTile = tile;

        UpdateSprite((HexDirection)Random.Range(0, 5));

        if (isHungry) {
            ChangeAnimation("Hungry", true);
        }
        else {
            ChangeAnimation("Idle", true);
        }
    }

    void EndMoving() {
        eventAfterMove?.Invoke();
        eventAfterMove = null;
        isMoving = false;
        /*
        tile.movingEntity = null;
        tile = movable.CurrentTile;
        tile.movingEntity = this;*/

        if (!harvestAnimation) {
            if (isHungry) {
                ChangeAnimation("Hungry", true);
            }
            else {
                ChangeAnimation("Idle", true);
            }
        }
        
        EndTurn();
    }


    public void EndEating(TrackEntry trackEntry) {
        if (isHungry) {
            ChangeAnimation("Hungry", true);
        }
        else {
            ChangeAnimation("Idle", true);
        }
        for (int i = 0; i < activatedSkeletons.Count; i++) {
            activatedSkeletons[i].state.Complete -= EndEating;
        }
        harvestAnimation = false;
    }

    public void SetPreviewTile(TileProperties aPreviewTile, bool colorTile = true) {
        previewTile.movablePreview = null;
        HexagonalGrid.Instance.Tilemap.SetColor(previewTile.Position, Color.white);

        previewTile = aPreviewTile;
        previewTile.movablePreview = movable;
        //if (colorTile)
           // HexagonalGrid.Instance.Tilemap.SetColor(aPreviewTile.Position, Color.red);
    }

    

    
   

}

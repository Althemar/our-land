using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[RequireComponent(typeof(Movable))]
public class MovingEntity : Entity
{
    private Movable movable;

    [HideInInspector]
    public MovingEntitySO movingEntitySO;

    public GameObject[] NW;
    public GameObject[] W;
    public GameObject[] SW;

    public GameObject NWContainer;
    public GameObject WContainer;
    public GameObject SWContainer;


    

    private EntityHungerState hunger;

    private Entity target;
    private bool stopBefore;
    private bool isMoving = false;
    [HideInInspector]
    public bool hasFled = false;

    private int baseSorting;

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
            tile.currentMovable = movable;
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

    public void UpdateSprite(HexDirection dir) {
        if (!NWContainer || !WContainer || !SWContainer)
            return;

        currentDir = dir;

        float flip = 1;
        if(dir == HexDirection.NE || dir == HexDirection.E || dir == HexDirection.SE)
            flip = -1;

        activatedSkeletons.Clear();
        UpdateSkeletons(NWContainer, NW, flip, dir, HexDirection.NW, HexDirection.NE);
        UpdateSkeletons(WContainer, W, flip, dir, HexDirection.W, HexDirection.E);
        UpdateSkeletons(SWContainer, SW, flip, dir, HexDirection.SW, HexDirection.SE);
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
    public void MoveTo(TileProperties to, Movable.OnMovableDelegate onEndMove) {
        var pathToTarget = AStarSearch.Path(tile, to, entitySO.availableTiles);
        if (pathToTarget != null && pathToTarget.Count >= 0) {
            TileProperties newTile = movable.MoveToward(pathToTarget, movingEntitySO.movementPoints, to.movingEntity != null);
            if(newTile) {
                tile.currentMovable = null;
                tile.movingEntity = null;
                eventAfterMove = onEndMove;
                tile = newTile;
                tile.movingEntity = this;
                isMoving = true;
            ChangeAnimation("Walk", true);
            }
            // TODO make end turn
        }
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
        tile.movingEntity = null;
        tile = movable.CurrentTile;
        tile.movingEntity = this;

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

    

    
   

}

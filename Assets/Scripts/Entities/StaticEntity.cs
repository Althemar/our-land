using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StaticEntity : Entity {
    [HideInInspector]
    public StaticEntitySO staticEntitySO;
    
    public List<GameObject> sprites;

    public List<SpriteRenderer> allSprites;
    public Transform spritesTransform;
    public int maxNumberOfSprites;
    public bool activateAllSprites;
    public float minScale, maxScale;

    GameObject activeSprite;

    protected override void Awake() {
        base.Awake();
        staticEntitySO = entitySO as StaticEntitySO;
        Initialize();
        transform.position = transform.position + new Vector3(0, 0, 1);
    }

    protected override void Start() {
        base.Start();
        OnPopulationChange += UpdateSprite;
    }



    void Destroy() {
        OnPopulationChange -= UpdateSprite;
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        EndTurn();
    }

    public void UpdateSprite() {
        if (activateAllSprites) {
            for (int i = 0; i < sprites.Count; i++) {
                if (sprites[i] && sprites[i].gameObject) {
                    sprites[i].gameObject.SetActive((i < population));
                }
            }
            return;
        }

        GameObject rendererToActivate;
        if (population <= 0) {
            rendererToActivate = null;
        }
        else {
            rendererToActivate = sprites[population - 1];
        }
        if (activeSprite != rendererToActivate && rendererToActivate) {
            activeSprite?.gameObject.SetActive(false);
            rendererToActivate.gameObject.SetActive(true);
            activeSprite = rendererToActivate;
        }
        else if (!rendererToActivate && activeSprite && population <= 0) {
            activeSprite.gameObject.SetActive(false);
            activeSprite = null;
        }

    }

    public override bool Initialize(int population = -1) {
        if (!base.Initialize(population))
            return false;
        
        GetComponent<SortingGroup>().sortingOrder = -tile.Position.y;

        for (int i = 0; i < sprites.Count; i++) {
            sprites[i].gameObject.SetActive(false);
            //transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.y);
        }
        tile.staticEntity = this;

        if (spritesTransform) {
            foreach (GameObject spritePop in sprites) {
                int numberOfSprites = Random.Range(1, maxNumberOfSprites + 1);
                while (numberOfSprites > 0) {
                    numberOfSprites--;
                    int index = Random.Range(0, allSprites.Count);
                    allSprites[index].transform.parent = spritePop.transform;

                    float scale = Random.Range(minScale, maxScale);
                    Vector3 scaleVector = new Vector3(scale, scale, 1);
                    allSprites[index].transform.localScale = scaleVector;
                    allSprites[index].GetComponent<SortingGroup>().sortingOrder = -tile.Position.y;
                    

                    allSprites.RemoveAt(index);
                }
                
            }
            for (int i = 0; i < allSprites.Count; i++) {
                Destroy(allSprites[i].gameObject);

            }
        }
        UpdateSprite();

        

        return true;
    }

    public override EntityType GetEntityType() {
        return EntityType.Static;
    }

    private void OnMouseEnter() {
        for (int i = 0; i < sprites.Count; i++) {
            foreach (SpriteRenderer renderer in sprites[i].GetComponentsInChildren<SpriteRenderer>()) {
                renderer.material.SetFloat("_Intensity", 0.5f);
            }
        }
        if (activateAllSprites) {
            for (int i = 0; i < allSprites.Count; i++) {
                if (allSprites[i] == null)
                    continue;
                allSprites[i].material.SetFloat("_Intensity", 0.5f);
            }
            return;
        }
    }

    private void OnMouseExit() {
        for (int i = 0; i < sprites.Count; i++) {
            foreach (SpriteRenderer renderer in sprites[i].GetComponentsInChildren<SpriteRenderer>()) {
                renderer.material.SetFloat("_Intensity", 0f);
            }
        }

        if (activateAllSprites) {
            for (int i = 0; i < allSprites.Count; i++) {
                if (allSprites[i] == null)
                    continue;
                allSprites[i].material.SetFloat("_Intensity", 0f);
            }
            return;
        }
    }
}

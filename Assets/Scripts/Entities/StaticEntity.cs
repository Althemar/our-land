﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : Entity {
    [HideInInspector]
    public StaticEntitySO staticEntitySO;

    public List<SpriteRenderer> sprites;

    SpriteRenderer activeSprite;

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
        SpriteRenderer rendererToActivate;
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
        
        for (int i = 0; i < sprites.Count; i++) {
            sprites[i].gameObject.SetActive(false);
            sprites[i].sortingOrder = -tile.Position.y;
        }
        tile.staticEntity = this;
        UpdateSprite();

        return true;
    }

    public override EntityType GetEntityType() {
        return EntityType.Static;
    }
}

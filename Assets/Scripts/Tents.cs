using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TentOnTile : SerializableDictionaryBase<CustomTile, SpritePairArray>
{
}

[Serializable]
public struct SpritePairArray
{
    public List<SpritePair> sprites;
}

[Serializable]
public struct SpritePair
{
    public Sprite sprite1;
    public Sprite sprite2;
}

public class Tents : MonoBehaviour
{
    [SerializeField]
    public TentOnTile tentOnTiles;

    public SpriteRenderer tentLeft;
    public SpriteRenderer tentRight;


    public MotherShip motherShip;
    public float fadeSpeed;

    private int state = 0;
    private bool displaying;
    private SpritePair pair;
    private Color tmpColor;
    private int maxState;

    private void Awake() {
        pair = new SpritePair();
    }

    private void Start() {
        maxState = tentOnTiles[motherShip.Movable.CurrentTile.Tile].sprites.Count - 1;
        UpdateTents();
        ShowTents();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            ToggleTents();
        }
    }

    public void IncreaseState() {
        if (state < maxState) {
            state++;
        }
        UpdateTents();
    }

    public void ResetState() {
        state = 0;
        UpdateTents();
    }

    public void UpdateTents() {
        if (tentOnTiles.ContainsKey(motherShip.Movable.CurrentTile.Tile)) {
            pair = tentOnTiles[motherShip.Movable.CurrentTile.Tile].sprites[state];
            tentLeft.sprite = pair.sprite1;
            tentRight.sprite = pair.sprite2;
        }
    }

    public void ToggleTents() {
        displaying = !displaying;
        StopAllCoroutines();
        StartCoroutine(FadeTents());
    }
    
    public void ShowTents() {
        if (!tentOnTiles.ContainsKey(motherShip.Movable.CurrentTile.Tile)) {
            return;
        }
        displaying = true;
        StopAllCoroutines();
        StartCoroutine(FadeTents());
    }

    public void HideTents() {
        displaying = false;
        StopAllCoroutines();
        StartCoroutine(FadeTents());
    }

    public IEnumerator FadeTents() {
        bool alpha = (displaying) ? (tentLeft.color.a < 1) : (tentLeft.color.a > 0);
        while (alpha) {
            tmpColor = tentLeft.color;
            tmpColor.a = (displaying) ? (tmpColor.a + fadeSpeed) : (tmpColor.a - fadeSpeed);
            tentLeft.color = tmpColor;
            tentRight.color = tmpColor;
            alpha = (displaying) ? (tentLeft.color.a < 1) : (tentLeft.color.a > 0);
            yield return null;
        }
    }
}



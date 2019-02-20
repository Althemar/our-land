using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sprites
{
    public Sprite normal;
    public Sprite shadow;
    public Sprite winter;
    public Sprite winterShadow;
}

public class TreeSprites : MonoBehaviour
{
    public SpriteRenderer normal;
    public SpriteRenderer shadow;

    Sprites sprites;

    public List<Sprites> spritesSamples;

    private void Start() {
       if (spritesSamples.Count > 0) {
            sprites = spritesSamples[Random.Range(0, spritesSamples.Count)];
            normal.sprite = sprites.normal;
            shadow.sprite = sprites.shadow;
        }
    }

    public void SetWinter() {
        normal.sprite = sprites.winter;
        shadow.sprite = sprites.winterShadow;
    }
    
}

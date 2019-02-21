using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct WinterChange
{
    public CustomTile tile;
    public bool destroyAddons;
}

[Serializable]
public class WinterTilesDic : SerializableDictionaryBase<CustomTile, WinterChange> { }

[Serializable]
public class ReplaceSprite : SerializableDictionaryBase<Sprite, Sprite> { }

public class Winter : MonoBehaviour
{
    [BoxGroup("Tiles")]
    [SerializeField]
    public WinterTilesDic tilesToReplace;

    [BoxGroup("Trees")]
    public List<EntitySO> treesSO;

    [BoxGroup("Rivers")]
    [SerializeField]
    public ReplaceSprite replaceRivers;
    [BoxGroup("Rivers")]
    [SerializeField]
    public ReplaceSprite replaceLakes;

    public ParticleSystem psSnow;
    public List<ParticleSystem> psSnowBurst;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            BeginWinter();
        }
    }

    public void BeginWinter() {
        foreach (ParticleSystem ps in psSnowBurst) {
            ps.Play();
        }
        psSnow.Play();
        
        StartCoroutine(WaitBeforeWinter());   
    }

    public IEnumerator WaitBeforeWinter() {
        yield return new WaitForSeconds(6f);
        SetWinter();
    }

    private void SetWinter() {
        AkSoundEngine.PostEvent("Play_Winter_Wind", gameObject);
        GameManager.Instance.winter = true;
        Camera.main.GetComponent<AudioAmbient>().StopAmbient();

        TileProperties tile;
        Tilemap tilemap = HexagonalGrid.Instance.Tilemap;
        SpriteRenderer spriteRenderer;

        for (int i = 0; i < HexagonalGrid.Instance.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < HexagonalGrid.Instance.tilesArray.GetLength(1); j++) {
                tile = HexagonalGrid.Instance.tilesArray[i, j];

                if (tile && tile.Tile && tilesToReplace.ContainsKey(tile.Tile)) {

                    // Remove old addons and borders
                    tile.RemoveBorders();
                    if (tilesToReplace[tile.Tile].destroyAddons)
                        tile.RemoveAddons();

                    // Reinitialize tile
                    tilemap.SetTile(tile.Position, tilesToReplace[tile.Tile].tile);
                    tile.InitializeCustomTile();
                    tile.SetAddon();
                    tile.SetBorders();

                    // Trees sprites
                    if (tile.staticEntity && treesSO.Contains(tile.staticEntity.entitySO)) {
                        foreach (GameObject sprite in tile.staticEntity.sprites) {
                            foreach (Transform tree in sprite.transform) {
                                tree.GetComponent<TreeSprites>().SetWinter();
                            }
                        }
                    }

                    // Update mountains
                    foreach (Transform child in tile.addonsGameObjects.transform) {
                        Mountain mountain = child.GetComponent<Mountain>();
                        if (mountain) {
                            mountain.SetWinter();
                        }
                    }

                    // Lakes and rivers
                    foreach (Transform child in tile.riversGameObjects.transform) {
                        spriteRenderer = child.GetComponent<SpriteRenderer>();
                        if (spriteRenderer) {
                            if (replaceRivers.ContainsKey(spriteRenderer.sprite)) {
                                spriteRenderer.sprite = replaceRivers[spriteRenderer.sprite];
                            }
                            else if (replaceLakes.ContainsKey(spriteRenderer.sprite)) {
                                spriteRenderer.sprite = replaceLakes[spriteRenderer.sprite];
                            }
                        }
                    }

                }
            }
        }
    }
}


using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class WinterTilesDic : SerializableDictionaryBase<CustomTile, CustomTile> { }

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


    private void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            BeginWinter();
        }
    }

    public void BeginWinter() {

        AkSoundEngine.PostEvent("Play_Winter_Wind", gameObject);

        TileProperties tile;
        Tilemap tilemap = HexagonalGrid.Instance.Tilemap;
        SpriteRenderer spriteRenderer;

        for (int i = 0; i < HexagonalGrid.Instance.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < HexagonalGrid.Instance.tilesArray.GetLength(1); j++) {
                tile = HexagonalGrid.Instance.tilesArray[i, j];

                if (tile && tile.Tile && tilesToReplace.ContainsKey(tile.Tile)) {

                    // Remove old addons and borders
                    tile.RemoveBorders();
                    tile.RemoveAddons();

                    // Reinitialize tile
                    tilemap.SetTile(tile.Position, tilesToReplace[tile.Tile]);
                    tile.InitializeCustomTile();
                    tile.SetAddon();

                    // Trees sprites
                    if (tile.staticEntity && treesSO.Contains(tile.staticEntity.entitySO)) {
                        foreach (SpriteRenderer sr in tile.staticEntity.sprites) {
                            foreach (Transform sprite in sr.transform) {
                                sprite.GetComponent<TreeSprites>().SetWinter();
                            }
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


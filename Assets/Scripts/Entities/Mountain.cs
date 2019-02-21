using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class MountainOnTile : SerializableDictionaryBase<CustomTile, SpritesShadowList> { }

[Serializable]
public class SpritesShadowList
{
    public List<Sprites> sprites;
}

public class Mountain : MonoBehaviour {

    public SpriteRenderer mountain;
    public SpriteRenderer shadow;

    private Sprites sprites;

    [SerializeField]
    public MountainOnTile mountains;


    private void Awake() {
        Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(cellPosition) + new Vector3(0, 0, 1);
        TileProperties tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        tile.asMountain = true;
        mountain.sortingOrder = -tile.Position.y;
        shadow.sortingOrder = -tile.Position.y;
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 1) {
            Vector3Int pos = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            CustomTile tile = HexagonalGrid.Instance.GetTile(pos).Tile;
            if (mountains.ContainsKey(tile)) {
                sprites = mountains[tile].sprites[Random.Range(0, mountains[tile].sprites.Count)];
                mountain.sprite = sprites.normal;
                shadow.sprite = sprites.shadow;
            }
        }
    }

    public void SetWinter() {
        mountain.sprite = sprites.winter;
        shadow.sprite = sprites.winterShadow;
    }

}

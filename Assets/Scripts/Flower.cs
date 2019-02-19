using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Butterfly butterfly;
    public SpriteRenderer spriteRenderer;
    public int maxNumberOfButterflies;
    
    void Start()
    {
        Vector3Int pos = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        TileProperties tile = HexagonalGrid.Instance.GetTile(pos);
        if (tile.asLake || tile.windOrigin) {
            Destroy(gameObject);
        }

        int numberOfButterflies = Random.Range(0, maxNumberOfButterflies + 1);
        while (numberOfButterflies > 0) {
            Instantiate(butterfly, transform.position, Quaternion.identity, transform);
            numberOfButterflies--;
        }

        spriteRenderer.sortingOrder = -pos.y - 1;
    }
}

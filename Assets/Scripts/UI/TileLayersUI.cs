using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLayersUI : MonoBehaviour
{
    public Color normalColor;

    public Color lowRiverColor;
    public Color highRiverColor;

    public Color lowWindColor;
    public Color highWindColor;

    public Color lowHumidityColor;
    public Color highHumidityColor;

    public void NormalLayer() {
        foreach (TileProperties tile in HexagonalGrid.Instance.tilesArray) {
            tile.hexagonLayer.color = normalColor;
            tile.Tilemap.SetColor(tile.Position, Color.white);
        }
    }

    public void RiverLayer() {
        /*float radius = HexagonalGrid.Instance.humidity.riverRadius;
        foreach (TileProperties tile in HexagonalGrid.Instance.tilesArray) {
            float riverInfluence = tile.humidity - tile.windDryness;
            tile.hexagonLayer.color = Color.Lerp(lowRiverColor, highRiverColor, riverInfluence / radius);
        }*/
    }

    public void WindLayer() {
        float maxDryness = WindManager.Instance.maxBaseDryness;
        foreach (TileProperties tile in HexagonalGrid.Instance.tilesArray) {
            tile.hexagonLayer.color = Color.Lerp(lowWindColor, highWindColor, (-tile.windDryness) / maxDryness);
        }
    }

    public void HumidityLayer() {
        /*float maxHumidity = HexagonalGrid.Instance.humidity.riverRadius;
        foreach (TileProperties tile in HexagonalGrid.Instance.tilesArray) {
            tile.hexagonLayer.color = Color.Lerp(lowHumidityColor, highHumidityColor, tile.humidity / maxHumidity);
        }*/
    }
    
}

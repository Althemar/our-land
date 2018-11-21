using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CustomEditor(typeof(HexagonalGrid), true)]
public class HexagonalGridCustom : Editor
{
    public HexagonalGrid grid { get { return (target as HexagonalGrid); } }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        
        if (GUILayout.Button("Refresh tiles")) {
            Tilemap tilemap = grid.GetComponent<Tilemap>();
            if (tilemap) {
                tilemap.RefreshAllTiles();
            }
        }
    }
}

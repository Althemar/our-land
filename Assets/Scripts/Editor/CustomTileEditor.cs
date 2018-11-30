using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(CustomTile), true)]
public class CustomTileEditor : Editor
{
    private ReorderableList centers;
    private ReorderableList bordersNW;
    private ReorderableList bordersW;
    private ReorderableList bordersSW;

    public CustomTile tile { get { return (target as CustomTile); } }

    static string[] headers = { "Centers", "Borders NW", "Borders W", "Borders SW" };

    int currentListIndex;

    const float imageSize = 50f;
    const float imageX = 20f;
    const float padding = 15f;

    public void OnEnable() {
        SetReorderableList(ref centers, ref tile.centers);
        SetReorderableList(ref bordersNW, ref tile.bordersNWEditor);
        SetReorderableList(ref bordersW, ref tile.bordersWEditor);
        SetReorderableList(ref bordersSW, ref tile.bordersSWEditor);

        InitDictionary(tile.bordersNW);
        InitDictionary(tile.bordersW);
        InitDictionary(tile.bordersSW);
    }

    public void SetReorderableList(ref ReorderableList rl, ref List<Sprite> tileList) {
        if (tileList == null)
            tileList = new List<Sprite>();

        rl = new ReorderableList(tileList, typeof(Sprite), false, true, true, true);
        rl.drawHeaderCallback = OnDrawHeader;
        rl.drawElementCallback = OnDrawElement;
        rl.onAddCallback = OnAddElement;
        rl.elementHeightCallback = GetElementHeight;
    }

    public void InitDictionary(Dictionary<CustomTile.TerrainType, List<Sprite>> dic) {
        if (dic == null) {
            dic = new Dictionary<CustomTile.TerrainType, List<Sprite>>();
            int terrainCount = Enum.GetNames(typeof(CustomTile.TerrainType)).Length;
            for (int i = 0; i < terrainCount; i++) {
                dic.Add((CustomTile.TerrainType)i, new List<Sprite>());
            }
        }
    }

    public override void OnInspectorGUI() {
        tile.go = EditorGUILayout.ObjectField("Game Object", tile.go, typeof(GameObject), false) as GameObject;
        tile.canWalkThrough = EditorGUILayout.Toggle("Can walk through", tile.canWalkThrough);
        tile.walkCost = EditorGUILayout.IntField("Walk Cost", tile.walkCost);

        EditorGUILayout.Space();

        for (int i = 0; i < 4; i++) {
            DisplayList(i);
        }
    }

    public void DisplayList(int index) {
        
        ReorderableList rl = GetRL(index);
        List<Sprite> l = GetList(index);
        if (rl != null && l != null) {
            currentListIndex = index;
            rl.DoLayoutList();
            EditorGUILayout.Space();
        }
    }

    public ReorderableList GetRL(int index = -1) {
        if (index == -1) {
            index = currentListIndex;
        }
        if (index == 0) {
            return centers;
        }
        else if (index == 1) {
            return bordersNW;
        }
        else if (index == 2) {
            return bordersW;
        }
        else {
            return bordersSW;
        }
    }

    public List<Sprite> GetList(int index = -1) {
        if (index == -1) {
            index = currentListIndex;
        }
        if (index == 0) {
            return tile.centers;
        }
        else if (index == 1) {
            return tile.bordersNWEditor;
        }
        else if (index == 2) {
            return tile.bordersWEditor;
        }
        else {
            return tile.bordersSWEditor;
        }
    }

    private void OnDrawHeader(Rect rect) {
        GUI.Label(rect, headers[currentListIndex]);
    }

    private void OnAddElement(ReorderableList list) {

        if (list == centers) {
            tile.centers.Add(null);
        }
        else if (list == bordersNW) {
            tile.bordersNWEditor.Add(null);
        }
        else if (list == bordersW) {
            tile.bordersWEditor.Add(null);
        }
        else if (list == bordersSW) {
            tile.bordersSWEditor.Add(null);
        }
    }

    private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused) {
        List<Sprite> sprites = GetList();
        Sprite sprite = sprites[index];

        rect.x += imageX;
        rect.y += padding / 2;
        rect.width = imageSize;
        rect.height = imageSize;

        EditorGUI.BeginChangeCheck();
        sprite = EditorGUI.ObjectField(rect, sprite, typeof(Sprite), false) as Sprite;
        if (EditorGUI.EndChangeCheck()) {
            sprites[index] = sprite;
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        
        if (sprite) {
            rect.x += imageSize + padding / 2;
            rect.width += 50;
            EditorGUI.LabelField(rect, sprite.name);

            rect.y += 18;
            rect.width = 100;
            float baseY = rect.y;
            int terrainCount = Enum.GetNames(typeof(CustomTile.TerrainType)).Length;
            for (int i = 0; i < terrainCount; i++) {
                if (i % 2 == 0 && i != 0) {
                    rect.x += 80;
                    rect.y = baseY;
                }
                EditorGUI.ToggleLeft(rect, ((CustomTile.TerrainType)i).ToString(), true);
                rect.y += 18;

            }
        }
        
        

        
        
        //EditorGUI.Toggle(rect, CustomTile.TerrainType.Grass.ToString(), true);*/
    }

    private float GetElementHeight(int index) {
        return imageSize + padding;
    }

}

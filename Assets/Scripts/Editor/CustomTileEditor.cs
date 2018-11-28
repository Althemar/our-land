using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

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
        SetReorderableList(ref bordersNW, ref tile.bordersNW);
        SetReorderableList(ref bordersW, ref tile.bordersW);
        SetReorderableList(ref bordersSW, ref tile.bordersSW);
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

    public override void OnInspectorGUI() {
        tile.go = EditorGUILayout.ObjectField("Game Object", tile.go, typeof(GameObject), false) as GameObject;
        tile.canWalkThrough = EditorGUILayout.Toggle("Can walk through", tile.canWalkThrough);
        tile.walkCost = EditorGUILayout.IntField("Walk Cost", tile.walkCost);
        tile.ambientRTPC = EditorGUILayout.TextField("Ambient RTPC", tile.ambientRTPC);

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
            return tile.bordersNW;
        }
        else if (index == 2) {
            return tile.bordersW;
        }
        else {
            return tile.bordersSW;
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
            tile.bordersNW.Add(null);
        }
        else if (list == bordersW) {
            tile.bordersW.Add(null);
        }
        else if (list == bordersSW) {
            tile.bordersSW.Add(null);
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
            
    }

    private float GetElementHeight(int index) {
        return imageSize + padding;
    }

}

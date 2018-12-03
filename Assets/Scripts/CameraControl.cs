﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Need Camera as children
public class CameraControl : MonoBehaviour {

    public float speed = 12f;
    public float maxSpeed = 25f;

    public Vector2 zoomLimit = new Vector2(5f, 25);
    public float zoomLerp = 0.02f;
    
    float zoomValue = 10f;
    float zoomTarget = 10f;
    Vector2 targetPosition = new Vector2(0, 0);

    Camera cam;
    public Tilemap tilemap;
    Bounds bounds;

    void Start () {
        cam = this.transform.GetChild(0).GetComponent<Camera>();
        targetPosition = this.transform.position;
        zoomTarget = cam.orthographicSize;

        tilemap.CompressBounds();
        bounds = tilemap.localBounds;
        bounds.center += new Vector3(1f, 0, 0);
        bounds.Expand(new Vector2(-7f, -2.5f));
    }

    void Update () {
        MoveCamera(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
        ZoomCamera(-Input.mouseScrollDelta.y * 1.5f);
        GetSoundAmbient();
    }

    void MoveCamera(float x, float y) {
        targetPosition += new Vector2(x, y) * Time.deltaTime * speed;
        targetPosition = LimitToBounds(targetPosition);

        Vector2 nextPosition = Vector2.MoveTowards(this.transform.position, targetPosition, Time.deltaTime * maxSpeed);
        nextPosition = LimitToBounds(nextPosition);

        this.transform.position = nextPosition;
    }

    Vector2 LimitToBounds(Vector2 vec) {
        if(vec.x > bounds.max.x - zoomValue * 16f/9f)
            vec.x = bounds.max.x - zoomValue * 16f/9f;
        if(vec.x < bounds.min.x + zoomValue * 16f/9f)
            vec.x = bounds.min.x + zoomValue * 16f/9f;

        if(vec.y > bounds.max.y - zoomValue)
            vec.y = bounds.max.y - zoomValue;
        if(vec.y < bounds.min.y + zoomValue)
            vec.y = bounds.min.y + zoomValue;

        return vec;
    }

    void ZoomCamera(float delta) {
        zoomTarget += delta;

        if (zoomTarget < zoomLimit.x)
            zoomTarget = zoomLimit.x;
        if (zoomTarget > zoomLimit.y)
            zoomTarget = zoomLimit.y;

        zoomValue = Mathf.Lerp(zoomValue, zoomTarget, zoomLerp);

        //ORTHOGRAPHIC CAMERA ONLY
        cam.orthographicSize = zoomValue;

        //Camera tilt + zoom (PERSPECTIVE CAMERA ONLY)
        //this.transform.rotation = Quaternion.AngleAxis (Mathf.Min (zoomValue - 13, 0), Vector3.right);
        //cam.transform.localPosition = new Vector3 (0, 0, -15 + zoomValue / 2f);
    }


    void GetSoundAmbient () {
        Ray rMin = cam.ViewportPointToRay (Vector3.zero);
        Ray rMax = cam.ViewportPointToRay (Vector3.one);
        Plane plane = new Plane (Vector3.forward, Vector3.zero);

        float distMin = 0.0f;
        float distMax = 0.0f;

        if (!plane.Raycast (rMin, out distMin))
            return;
        if (!plane.Raycast (rMax, out distMax))
            return;

        Vector3 minPoint = rMin.GetPoint (distMin);
        Vector3Int cellMin = tilemap.WorldToCell (minPoint);

        Vector3 maxPoint = rMax.GetPoint (distMax);
        Vector3Int cellMax = tilemap.WorldToCell (maxPoint);
        cellMax.x++;
        cellMax.y++;
        cellMax.z = 1;

        BoundsInt b = new BoundsInt ();
        b.min = cellMin;
        b.max = cellMax;

        Dictionary<string, int> ambientRTPCs = new Dictionary<string, int>();
        int total = 0;
        foreach (Vector3Int p in b.allPositionsWithin) {
            CustomTile tileData = (tilemap.GetTile(p) as CustomTile);
            if(!ambientRTPCs.ContainsKey(tileData.ambientRTPC))
                ambientRTPCs.Add(tileData.ambientRTPC, 0);
            ambientRTPCs[tileData.ambientRTPC]++;
            total++;
        }

        foreach(var d in ambientRTPCs) {
            AkSoundEngine.SetRTPCValue(d.Key, (float)d.Value / (float) total * 100.0f);
        }
    }
}
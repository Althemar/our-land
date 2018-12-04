using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AudioAmbient : MonoBehaviour {
    Camera cam;
    public Tilemap tilemap;
    bool musicPlayed = false;

    float timeBetweenMusics = Mathf.Infinity;

    void Start () {
        cam = this.GetComponent<Camera> ();
    }

    // Update is called once per frame
    void Update () {
        GetSoundAmbient();
        PlayMusic();
    }

    void PlayMusic() {
        if(musicPlayed)
            return;
        
        timeBetweenMusics += Time.deltaTime;
        if(timeBetweenMusics < GameSettings.silence)
            return;

        musicPlayed = true;
        AkSoundEngine.PostEvent("Play_MUSIC", this.gameObject, (uint)AkCallbackType.AK_MusicSyncExit, MyCallbackFunction, null);
    }

    void MyCallbackFunction(object in_cookie, AkCallbackType in_type, object in_info) {
        if (in_type == AkCallbackType.AK_MusicSyncExit) {
            musicPlayed = false;
            timeBetweenMusics = 0.0f;
        }
    }

    void GetSoundAmbient() {
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

        Dictionary<string, int> ambientRTPCs = new Dictionary<string, int> ();
        int total = 0;
        foreach (Vector3Int p in b.allPositionsWithin) {
            CustomTile tileData = (tilemap.GetTile (p) as CustomTile);
            if (!ambientRTPCs.ContainsKey (tileData.ambientRTPC))
                ambientRTPCs.Add (tileData.ambientRTPC, 0);
            ambientRTPCs[tileData.ambientRTPC]++;
            total++;
        }

        foreach (var d in ambientRTPCs) {
            AkSoundEngine.SetRTPCValue (d.Key, (float) d.Value / (float) total * 100.0f);
        }
    }
}
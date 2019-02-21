using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class AudioTile {
    public int num;
    public Vector2 sumPos;
}

public class AudioAmbient : MonoBehaviour {
    Camera cam;
    private HexagonalGrid grid;
    bool musicPlayed = false;

    float timeBetweenMusics = Mathf.Infinity;

    [ConfigVar(Name = "audio.silence", DefaultValue = "5", Description = "Silence between musics")]
    static ConfigVar silenceBetweenMusic;

    void Start() {
        cam = this.GetComponent<Camera>();

        grid = HexagonalGrid.Instance;

        AkSoundEngine.SetRTPCValue("AMBIANCE_VOLUME_DESERT", 0);
        AkSoundEngine.SetRTPCValue("AMBIANCE_VOLUME_PRAIRIE", 0);
        AkSoundEngine.SetRTPCValue("AMBIANCE_VOLUME_WATER", 0);
        AkSoundEngine.SetRTPCValue("AMBIANCE_VOLUME_WETLANDS", 0);

        AkSoundEngine.PostEvent("Play_DESERT_Pl", this.gameObject);
        AkSoundEngine.PostEvent("Play_PRAIRIE_Pl", this.gameObject);
        AkSoundEngine.PostEvent("Play_WATER_Pl", this.gameObject);
        AkSoundEngine.PostEvent("Play_WETLAND_Pl", this.gameObject);
    }

    // Update is called once per frame
    void Update() {
        GetSoundAmbient();
        PlayMusic();
    }

    void PlayMusic() {
        if (musicPlayed)
            return;

        timeBetweenMusics += Time.deltaTime;
        if (timeBetweenMusics < silenceBetweenMusic.FloatValue)
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

        if (GameManager.Instance.winter) {
            return;
        }

        Ray rMin = cam.ViewportPointToRay(Vector3.zero);
        Ray rMax = cam.ViewportPointToRay(Vector3.one);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        float distMin = 0.0f;
        float distMax = 0.0f;

        if (!plane.Raycast(rMin, out distMin))
            return;
        if (!plane.Raycast(rMax, out distMax))
            return;

        Vector3 minPoint = rMin.GetPoint(distMin);
        Vector3Int cellMin = grid.Tilemap.WorldToCell(minPoint);

        Vector3 maxPoint = rMax.GetPoint(distMax);
        Vector3Int cellMax = grid.Tilemap.WorldToCell(maxPoint);
        cellMax.x++;
        cellMax.y++;
        cellMax.z = 1;

        BoundsInt b = new BoundsInt();
        b.min = cellMin;
        b.max = cellMax;

        Dictionary<string, AudioTile> ambientRTPCs = new Dictionary<string, AudioTile>();
        ambientRTPCs.Add("DESERT", new AudioTile() { num = 0 });
        ambientRTPCs.Add("PRAIRIE", new AudioTile() { num = 0 });
        ambientRTPCs.Add("WATER", new AudioTile() { num = 0 });
        ambientRTPCs.Add("WETLANDS", new AudioTile() { num = 0 });
        ambientRTPCs.Add("RIVER", new AudioTile() { num = 0 });

        
        int total = 0;
        foreach (Vector3Int p in b.allPositionsWithin) {
            TileProperties prop = grid.GetTile(p);
            if (!prop)
                continue;
            CustomTile tileData = prop.Tile;
            if (!tileData)
                continue;

            Vector2 posCam = new Vector2(
                Mathf.InverseLerp(b.min.x, b.max.x - 1, p.x),
                Mathf.InverseLerp(b.min.y, b.max.y - 1, p.y)
            );

            if (prop.asRiver) {
                ambientRTPCs["RIVER"].num++;
                ambientRTPCs["RIVER"].sumPos += posCam;

                total++;
            }
            if (prop.asLake) {
                ambientRTPCs["WATER"].num++;
                ambientRTPCs["WATER"].sumPos += posCam;

                total++;
            }

            if (tileData.ambientRTPC == "")
                continue;

            ambientRTPCs[tileData.ambientRTPC].num++;
            ambientRTPCs[tileData.ambientRTPC].sumPos += posCam;

            total++;
        }

        foreach (var d in ambientRTPCs) {
            AkSoundEngine.SetRTPCValue("AMBIANCE_VOLUME_" + d.Key, (float)d.Value.num / (float)total * 100.0f);
            if (d.Value.num != 0)
                AkSoundEngine.SetRTPCValue("AMBIANCE_SPAT_" + d.Key, (d.Value.sumPos / d.Value.num).x);
        }
    }

    public void StopAmbient() {

        Dictionary<string, AudioTile> ambientRTPCs = new Dictionary<string, AudioTile>();
        ambientRTPCs.Add("DESERT", new AudioTile() { num = 0 });
        ambientRTPCs.Add("PRAIRIE", new AudioTile() { num = 0 });
        ambientRTPCs.Add("WATER", new AudioTile() { num = 0 });
        ambientRTPCs.Add("WETLANDS", new AudioTile() { num = 0 });
        ambientRTPCs.Add("RIVER", new AudioTile() { num = 0 });

        foreach (var d in ambientRTPCs) {
            AkSoundEngine.SetRTPCValue("AMBIANCE_VOLUME_" + d.Key, 0);
            if (d.Value.num != 0)
                AkSoundEngine.SetRTPCValue("AMBIANCE_SPAT_" + d.Key, 0);
        }
    }
}
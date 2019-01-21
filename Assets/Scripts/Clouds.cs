using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour {
    public Sprite[] clouds;
    public Material productShader;
    public CameraControl cameraman;

    List<SpriteRenderer> cloudObjects;

    // Start is called before the first frame update
    void Start() {
        cloudObjects = new List<SpriteRenderer>();

        for(int i = 0; i < 10; i++)
            CreateCloud();
    }

    // Update is called once per frame
    void Update() {
        float val = cameraman.GetZoomValue();
        foreach (SpriteRenderer renderer in cloudObjects) {
            renderer.color = new Color(1, 1, 1, val / 1.5f);
            renderer.transform.position += new Vector3(-0.3f, -0.1f) * renderer.transform.position.z * Time.deltaTime;
        }
    }

    void CreateCloud() {
        GameObject obj = new GameObject();
        obj.transform.parent = this.transform;
        obj.transform.position = new Vector3(Random.Range(-100f, 100f), Random.Range(-60f, 60f), Random.Range(1f, 10f));
        float size = Random.Range(2.5f, 5.5f);
        obj.transform.localScale = new Vector3(size, size, size);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = clouds[Random.Range(0, clouds.Length)];
        renderer.sortingOrder = 50;
        renderer.material = productShader;
        renderer.color = new Color(1, 1, 1, 0.5f);
        cloudObjects.Add(renderer);
    }
}

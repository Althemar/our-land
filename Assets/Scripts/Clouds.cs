using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour {
    public Sprite[] clouds;
    public Material productShader;
    public CameraControl cameraman;

    public Bounds bounds;

    List<SpriteRenderer> cloudObjects;

    // Start is called before the first frame update
    void Start() {
        cloudObjects = new List<SpriteRenderer>();

        for(int i = 0; i < 40; i++)
            CreateCloud();
    }

    // Update is called once per frame
    void Update() {
        float val = cameraman.GetZoomValue();
        foreach (SpriteRenderer renderer in cloudObjects) {
            renderer.color = new Color(1, 1, 1, val);
            renderer.transform.localPosition += new Vector3(-0.3f, -0.1f) * renderer.transform.position.z * Time.deltaTime;
            //if (renderer.transform.position.x < 10)
            if(renderer.transform.localPosition.y < bounds.min.y) {
                renderer.transform.localPosition = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y + 5, Random.Range(1f, 10f));
            }
        }
    }

    void CreateCloud() {
        GameObject obj = new GameObject();
        obj.layer = 13;
        obj.transform.parent = this.transform;
        obj.transform.localPosition = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(1f, 10f));
        float size = Random.Range(2.5f, 5.5f);
        obj.transform.localScale = new Vector3(size, size, size);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = clouds[Random.Range(0, clouds.Length)];
        renderer.sortingOrder = 200;
        renderer.material = productShader;
        renderer.color = new Color(1, 1, 1, 0.3f);
        cloudObjects.Add(renderer);
    }

    void OnDrawGizmosSelected() {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawLine(this.transform.position + new Vector3(bounds.min.x, bounds.min.y), this.transform.position + new Vector3(bounds.max.x, bounds.min.y));
        Gizmos.DrawLine(this.transform.position + new Vector3(bounds.max.x, bounds.min.y), this.transform.position + new Vector3(bounds.max.x, bounds.max.y));
        Gizmos.DrawLine(this.transform.position + new Vector3(bounds.max.x, bounds.max.y), this.transform.position + new Vector3(bounds.min.x, bounds.max.y));
        Gizmos.DrawLine(this.transform.position + new Vector3(bounds.min.x, bounds.max.y), this.transform.position + new Vector3(bounds.min.x, bounds.min.y));
    }
}

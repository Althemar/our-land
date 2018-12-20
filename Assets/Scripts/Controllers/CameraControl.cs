using System.Collections;
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

    // UGLY, in the future: allow traget any entity
    public MotherShip playerShip;
    bool follow;

    void Start () {
        cam = this.transform.GetChild(0).GetComponent<Camera>();
        targetPosition = this.transform.position;
        zoomTarget = cam.orthographicSize;

        tilemap.CompressBounds();
        bounds = tilemap.localBounds;
        bounds.center += new Vector3(1f, 0, 0);
        bounds.Expand(new Vector2(-7f, -2.5f));

        AkSoundEngine.PostEvent("Play_WIND", this.gameObject);

        playerShip.OnBeginMoving += FollowEntity;
        playerShip.OnEndMoving += EndFollowEntity;
    }

    void FollowEntity() {
        follow = true;
    }

    void EndFollowEntity() {
        follow = false;
    }

    void Update () {
        if(follow)
            targetPosition = playerShip.transform.position;
        Vector2 movementCam = new Vector2(GameManager.Input.GetAxis ("Horizontal"), GameManager.Input.GetAxis ("Vertical"));
        if(GameManager.Input.mousePosition.x < 5)
            movementCam.x -= 0.75f;
        if(GameManager.Input.mousePosition.x > Screen.width - 5)
            movementCam.x += 0.75f;
        if(GameManager.Input.mousePosition.y < 5)
            movementCam.y -= 0.75f;
        if(GameManager.Input.mousePosition.y > Screen.height - 5)
            movementCam.y += 0.75f;
        MoveCamera(movementCam.x, movementCam.y);
        ZoomCamera(-GameManager.Input.mouseScrollDelta.y * 1.5f);
        
        AkSoundEngine.SetRTPCValue("AMBIANCE_WIND_MOD", Mathf.InverseLerp(zoomLimit.x, zoomLimit.y, zoomValue) * 100f);
        AkSoundEngine.SetRTPCValue("ZOOM_LEVEL", Mathf.InverseLerp(zoomLimit.x, zoomLimit.y, zoomValue) * 100f);
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

    public float GetZoomValue() {
        return Mathf.InverseLerp(zoomLimit.x, zoomLimit.y, zoomValue);
    }

}
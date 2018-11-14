using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMetrics{

    public float outerRadius;
    public float innerRadius;
    public Vector3[] corners = new Vector3[6];
    public Vector3[] edgesMiddle = new Vector3[6];

    public static Vector3[] directions = { new Vector3Int(1, 1, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0),
                                           new Vector3Int(-1, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 0)
                                         };

    public HexMetrics(float outer) {
        outerRadius = outer;
        innerRadius = outer * 0.866025404f;
        corners[0] = new Vector3(0f, outerRadius, 0f);
        corners[1] = new Vector3(innerRadius, 0.5f * outerRadius, 0f);
        corners[2] = new Vector3(innerRadius, -0.5f * outerRadius, 0f);
        corners[3] = new Vector3(0f, -outerRadius, 0f);
        corners[4] = new Vector3(-innerRadius, -0.5f * outerRadius, 0f);
        corners[5] = new Vector3(-innerRadius, 0.5f * outerRadius, 0f);
    }

    public Vector3 GetCorner(bool right, HexDirection direction) {
        if (!right) {
            return corners[(int)direction];
        }
        else {
            if (direction != HexDirection.NW) {
                return corners[(int)direction + 1];
            }
            else {
                return corners[0];
            }
        }
    }
}

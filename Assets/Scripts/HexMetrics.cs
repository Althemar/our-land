using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMetrics{
    
    public Vector3[] corners = new Vector3[6];

    public HexMetrics(float outerRadius, float innerRadius) {
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

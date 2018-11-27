using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMetrics{
    
    Vector3[] corners = new Vector3[6];
    Vector3[] borders = new Vector3[6];

    float outerRadius;
    float innerRadius;

    float height;
    float width;


    public HexMetrics(float outerRadius, float innerRadius) {
        CalculateCorners(outerRadius, innerRadius);
    }

    public HexMetrics(float outerRadius) {
        CalculateCorners(outerRadius, outerRadius * 0.866025404f);
        
    }

    private void CalculateCorners(float outerRadius, float innerRadius) {
        this.outerRadius = outerRadius;
        this.innerRadius = innerRadius;
        height = outerRadius / 2;
        width = innerRadius / 2;

        corners[0] = new Vector3(0f, outerRadius, 0f);
        corners[1] = new Vector3(innerRadius, 0.5f * outerRadius, 0f);
        corners[2] = new Vector3(innerRadius, -0.5f * outerRadius, 0f);
        corners[3] = new Vector3(0f, -outerRadius, 0f);
        corners[4] = new Vector3(-innerRadius, -0.5f * outerRadius, 0f);
        corners[5] = new Vector3(-innerRadius, 0.5f * outerRadius, 0f);

        CalculateBorders();
    }

    private void CalculateBorders() {
        borders[0] = new Vector3(innerRadius, 0.75f * outerRadius, 0f) / 2;
        borders[1] = new Vector3(width, 0f, 0f) / 2;
        borders[2] = new Vector3(innerRadius, - 0.75f * outerRadius, 0f) / 2;
        borders[3] = new Vector3(-innerRadius, -0.75f * outerRadius, 0f) / 2;
        borders[4] = new Vector3(-width, 0f, 0f) / 2;
        borders[5] = new Vector3(-innerRadius, 0.75f * outerRadius, 0f) / 2;

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

    public Vector3 GetCorner(int index) {
        return corners[index];
    }

    public Vector3 GetBorder(int index) {
        return borders[index];
    }
}

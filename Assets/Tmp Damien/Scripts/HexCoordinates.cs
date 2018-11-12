using UnityEngine;

/*
 * HexCoordinates
 * Coordinates for an hexagonal grid
 * Can be converted in offset, axial, or cubic coordinates
 */

public class HexCoordinates
{
    public int x, y, z;
    public HexCoordinatesType coordinatesType;

    public HexCoordinates(int x, int y) {
        this.x = x;
        this.y = y;
        z = 0;
        coordinatesType = HexCoordinatesType.offset;
    }

    public HexCoordinates(Vector3Int position, HexCoordinatesType coordinatesType = HexCoordinatesType.cubic) {
        x = position.x;
        y = position.y;
        z = position.z;
        this.coordinatesType = coordinatesType;
    }

    public HexCoordinates(HexCoordinatesType coordinatesType) {
        this.coordinatesType = coordinatesType;
    }

    public void ChangeCoordinatesType(HexCoordinatesType type) {
        switch (type) {
            case HexCoordinatesType.offset:
                ToOffset();
                break;
            case HexCoordinatesType.axial:
                ToAxial();
                break;
            case HexCoordinatesType.cubic:
                ToCubic();
                break;
        }
    }

    void ToOffset() {
        switch (coordinatesType) {
            case HexCoordinatesType.axial:
                AxialToOffset();
                break;
            case HexCoordinatesType.cubic:
                CubicToOffset();
                break;
        }
    }

    void ToAxial() {
        switch (coordinatesType) {
            case HexCoordinatesType.offset:
                OffsetToAxial();
                break;
            case HexCoordinatesType.cubic:
                CubicToAxial();
                break;
        }
    }

    void ToCubic() {
        switch (coordinatesType) {
            case HexCoordinatesType.axial:
                AxialToCubic();
                break;
            case HexCoordinatesType.offset:
                OffsetToCubic();
                break;
        }
    }

    void OffsetToCubic() {
        OffsetToAxial();
        AxialToCubic();
    }

    void CubicToOffset() {
        CubicToAxial();
        AxialToOffset();
    }

    void OffsetToAxial() {
        x = x - (y - (y & 1)) / 2;
        coordinatesType = HexCoordinatesType.axial;
    }

    void AxialToOffset() {
        x = x + (y - (y & 1)) / 2;
        coordinatesType = HexCoordinatesType.offset;
    }

    void  AxialToCubic() {
        z = -y - x;
        coordinatesType = HexCoordinatesType.cubic;
    }

    void CubicToAxial() {
        z = 0;
        coordinatesType = HexCoordinatesType.axial;
    }

    public HexCoordinates GetCoordinatesOfType(HexCoordinatesType type) {
        if (coordinatesType != type) {
            HexCoordinates newCoordinates = this;
            newCoordinates.ChangeCoordinatesType(type);
            return newCoordinates;
        }
        else {
            return this;
        }
    }

    public Vector3Int ToVector3Int() {
        return new Vector3Int(x, y, z);
    }

    public static HexCoordinates operator +(HexCoordinates c1, HexCoordinates c2) {
        if (c1.coordinatesType == c2.coordinatesType) {
            HexCoordinates c = new HexCoordinates(c1.coordinatesType);
            c.x = c1.x + c2.x;
            c.y = c1.y + c2.y;
            c.z = c1.z + c2.z;
            return c;
        }
        else {
            return null;
        }
    }

    public int Distance(HexCoordinates c2) {
        if (coordinatesType != HexCoordinatesType.cubic) {
            ChangeCoordinatesType(HexCoordinatesType.cubic);
        }
        if (c2.coordinatesType != HexCoordinatesType.cubic) {
            c2.ChangeCoordinatesType(HexCoordinatesType.cubic);
        }
        return (Mathf.Abs(x - c2.x) + Mathf.Abs(y - c2.y) + Mathf.Abs(z - c2.z)) / 2;
    }
}


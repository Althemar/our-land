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

    public HexCoordinates(Vector3Int position) {
        x = position.x;
        y = position.y;
        z = position.z;
        coordinatesType = HexCoordinatesType.cubic;
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

    public static HexCoordinates GetOffsetCoordinates(HexCoordinates coordinates) {
        HexCoordinates newCoordinates = coordinates;
        coordinates.ChangeCoordinatesType(HexCoordinatesType.offset);
        return coordinates;
    }

    public static HexCoordinates GetCubicCoordinates(HexCoordinates coordinates) {
        HexCoordinates newCoordinates = coordinates;
        coordinates.ChangeCoordinatesType(HexCoordinatesType.cubic);
        return coordinates;
    }

    public Vector3Int toVector3Int() {
        return new Vector3Int(x, y, z);
    }
}


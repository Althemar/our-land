using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HexCoordinates
{
    public enum HexCoordinatesType { offset, axial, cubic}

    public int x, y, z;
    public HexCoordinatesType coordinatesType;

    public HexCoordinates(int x, int y) {
        this.x = x;
        this.y = y;
        coordinatesType = HexCoordinatesType.offset;
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
        coordinatesType = HexCoordinatesType.axial;
    }
}


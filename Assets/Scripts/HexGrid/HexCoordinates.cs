using UnityEngine;

/*
 * HexCoordinates
 * Coordinates for an hexagonal grid
 * Can be converted in offset, axial, or cubic coordinates
 */

public class HexCoordinates
{
    private Vector3Int axial, cubic, offset;

    public Vector3Int AxialCoordinates { get => axial; }
    public Vector3Int CubicCoordinates { get => cubic; }
    public Vector3Int OffsetCoordinates { get => offset; }

    public HexCoordinates(int x, int y) {
        this.offset = new Vector3Int(x, y, 0);
        this.axial = OffsetToAxial(this.offset);
        this.cubic = AxialToCubic(this.axial);
    }

    public HexCoordinates(int x, int y, int z) {
        this.cubic = new Vector3Int(x, y, z);
        this.axial = CubicToAxial(this.cubic);
        this.offset = AxialToOffset(this.axial);
    }

    public HexCoordinates(Vector3Int cubeCoord) {
        this.cubic = cubeCoord;
        this.offset = CubicToAxial(this.cubic);
        this.axial = AxialToOffset(this.axial);
    }

    public HexCoordinates(HexCoordinates h) {
        this.offset = h.offset;
        this.axial = h.axial;
        this.cubic = h.cubic;
    }

    static Vector3Int OffsetToCubic(Vector3Int offset) {
        Vector3Int axial = OffsetToAxial(offset);
        return AxialToCubic(axial);
    }

    static Vector3Int CubicToOffset(Vector3Int cubic) {
        Vector3Int axial = CubicToAxial(cubic);
        return AxialToOffset(axial);
    }

    static Vector3Int OffsetToAxial(Vector3Int offset) {
        Vector3Int res = offset;
        res.x = offset.x - (offset.y - (offset.y & 1)) / 2;
        return res;
    }

    static Vector3Int AxialToOffset(Vector3Int axial) {
        Vector3Int res = axial;
        res.x = axial.x + (axial.y - (axial.y & 1)) / 2;
        return res;
    }

    static Vector3Int AxialToCubic(Vector3Int axial) {
        Vector3Int res = axial;
        res.z = -axial.y - axial.x;
        return res;
    }

    static Vector3Int CubicToAxial(Vector3Int cubic) {
        Vector3Int res = cubic;
        res.z = 0;
        return res;
    }

    public static HexCoordinates operator+(HexCoordinates c1, HexCoordinates c2) {
        HexCoordinates c = new HexCoordinates(c1);

        c.cubic += c2.cubic;
        c.axial = CubicToAxial(c.cubic);
        c.offset = AxialToOffset(c.axial);

        return c;
    }

    public static HexCoordinates operator-(HexCoordinates c1, HexCoordinates c2) {
        HexCoordinates c = new HexCoordinates(c1);
        c.cubic -= c2.cubic;
        c.axial = CubicToAxial(c.cubic);
        c.offset = AxialToOffset(c.axial);
        return c;
    }

    // Get the distance between two cubic coordinates
    public int Distance(HexCoordinates c2) {
        return (Mathf.Abs(cubic.x - c2.cubic.x) + Mathf.Abs(cubic.y - c2.cubic.y) + Mathf.Abs(cubic.z - c2.cubic.z)) / 2;
    }

    public HexCoordinates Opposite(HexCoordinates c2) {
        HexCoordinates difference = c2 - this;
        HexCoordinates opposite = this - difference;
        return opposite;
    }
}


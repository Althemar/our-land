using System;
using UnityEngine;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction) {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction, int minus = 1) {
        int count = Enum.GetNames(typeof(HexDirection)).Length;
        return (int)direction > (minus - 1) ? (direction - minus) : (direction + (count-minus));
    }

    public static HexDirection Next(this HexDirection direction) {
        return (int)direction < 5 ? (direction + 1) : (0);
    }

   
}
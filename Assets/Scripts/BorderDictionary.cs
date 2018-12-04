using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BorderDictionary : SerializableDictionaryBase<CustomTile.TerrainType, SpriteList>
{

}

[Serializable]
public class SpriteList
{
    public List<Sprite> sprites;
}

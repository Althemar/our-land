using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class I18nData : ScriptableObject {
    [SerializeField]
    public List<I18nTextSave> i18nData;
}

[System.Serializable]
public struct I18nTextSave {
    [SerializeField]
    public string key;
    [SerializeField]
    public string fr;
    [SerializeField]
    public string en;
}
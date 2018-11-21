using System;
using System.Collections.Generic;
using UnityEngine;

public static class I18N {

    public struct I18nText {
        public string fr;
        public string en;
    }

    public enum Lang {
        FR, EN
    };

    public static Dictionary<string, I18nText> i18n;
    public static bool initialized = false;

    public static string GetText(string key) {
        Initialize();

        if(!i18n.ContainsKey(key))
            return "_" + key;
        
        return i18n[key].fr;
    }

    public static void Initialize() {
        if(initialized)
            return;

        Debug.Log("Init!");
        LoadText();
        initialized = true;
    }

    public static void LoadText() {
        Debug.Log("Load!");
        I18nData data = Resources.Load<I18nData>("i18n/Data");
        i18n = new Dictionary<string, I18nText>();
        foreach (I18nTextSave l in data.i18nData) {
            i18n.Add(l.key, new I18nText(){fr = l.fr, en = l.en});
        }
    }

}
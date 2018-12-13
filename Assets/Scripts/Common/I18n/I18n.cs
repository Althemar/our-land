using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void CallbackFunction();

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
    public static Lang lang = Lang.FR;

    public static CallbackFunction OnLangChange;

    public static string GetText(string key) {
        Initialize();

        if(!i18n.ContainsKey(key))
            return "_" + key;
        
        if(lang == Lang.FR)
            return i18n[key].fr;
        return i18n[key].en;
    }

    public static void Initialize() {
        if(initialized)
            return;
        
        LoadText();
        initialized = true;
    }

    public static void LoadText() {
        i18n = new Dictionary<string, I18nText>();

        I18nData data = Resources.Load<I18nData>("i18n/Data");
        if(data == null)
            return;
            
        foreach (I18nTextSave l in data.i18nData) {
            i18n.Add(l.key, new I18nText(){fr = l.fr, en = l.en});
        }
    }

    public static void ChangeLang(Lang l) {
        lang = l;
        OnLangChange();
    }

}
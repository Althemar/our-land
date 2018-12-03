using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

static class GameSettings {
    public static int silence = 90;
}

public class OptionMenu : MonoBehaviour {
    I18N.Lang prevLang;

    public TMP_Dropdown langDropdown;
    public Slider silenceSlider;

    public void OnEnable () {
        var backupEvent = langDropdown.onValueChanged;
        langDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent ();
        langDropdown.value = (int) I18N.lang;
        langDropdown.RefreshShownValue ();
        langDropdown.onValueChanged = backupEvent;

        silenceSlider.value = GameSettings.silence / (float)silenceSlider.maxValue;

        prevLang = I18N.lang;
    }

    public void ChangeLang (int value) {
        I18N.ChangeLang ((I18N.Lang) value);
    }

    public void Cancel () {
        I18N.ChangeLang (prevLang);
        Destroy (gameObject);
    }

    public void Apply () {
        Destroy (gameObject);
    }
}
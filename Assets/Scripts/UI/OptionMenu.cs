using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionMenu : MonoBehaviour {
    I18N.Lang prevLang;

    public TMP_Dropdown langDropdown;

    public void OnEnable () {
        var backupEvent = langDropdown.onValueChanged;
        langDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent ();
        langDropdown.value = (int) I18N.lang;
        langDropdown.RefreshShownValue ();
        langDropdown.onValueChanged = backupEvent;

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
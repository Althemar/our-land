using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button backButton;
    public Image panel;
    public float fadeSpeed;

    private Menu currentMenu;
    private Color tmpColor;

    public static MainMenu Instance;

    private void Start() {
        Instance = this;
        backButton.gameObject.SetActive(false);
        AkSoundEngine.PostEvent("Play_OurLand_MusicMenu", gameObject);
    }

    public void Play() {
        panel.gameObject.SetActive(true);
        AkSoundEngine.PostEvent("Stop_OurLand_MusicMenu", gameObject);
        StartCoroutine(FadeMenu());
    }

    public void SetCurrentMenu(Menu menu) {
        currentMenu = menu;
    }

    public void ToggleBackButton(bool toggle) {
        backButton.gameObject.SetActive(toggle);
    }

    public void PreviousMenu() {
        currentMenu.previous.gameObject.SetActive(true);
        currentMenu.gameObject.SetActive(false);
        currentMenu = currentMenu.previous;
        ToggleBackButton(currentMenu.backMenu);
    }

    public IEnumerator FadeMenu() {
        while (panel.color.a < 1) {
            tmpColor = panel.color;
            tmpColor.a += fadeSpeed * Time.deltaTime;
            panel.color = tmpColor;
            yield return null;
        }
        // SceneManager.LoadScene("Damien");
    }
}

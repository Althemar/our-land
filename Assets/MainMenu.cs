using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private Menu currentMenu;

    public static MainMenu Instance;

    private void Start() {
        Instance = this;
    }

    public void Play() {
        SceneManager.LoadScene("Damien");
    }

    public void SetCurrentMenu(Menu menu) {
        currentMenu = menu;
    }

    public void PreviousMenu() {
        currentMenu.previous.gameObject.SetActive(true);
        currentMenu.gameObject.SetActive(false);
        currentMenu = currentMenu.previous;
    }
}

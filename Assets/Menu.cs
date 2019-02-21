using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public List<Sprite> sprites;
    public Image image;

    [HideInInspector]
    public Menu previous;

    public void ChangeSprite(int index) {
        image.sprite = sprites[index];
    }

    public void Activate(Menu previous) {
        this.previous = previous;
        previous.gameObject.SetActive(false);
        gameObject.SetActive(true);
        MainMenu.Instance.SetCurrentMenu(this);
    }

   
}

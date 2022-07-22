using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject endGameMenu;
    public Text loseText;
    public Text winText;

    public void onOpenWinGameMenu()
    {
        endGameMenu.SetActive(true);
        loseText.enabled = false;
        winText.enabled = true;
    }

    public void onOpenLoseGameMenu()
    {
        endGameMenu.SetActive(true);
        loseText.enabled = true;
        winText.enabled = false;
    }

    public void onCloseEgameMenu()
    {
        endGameMenu.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCanvas : UICanvas
{
    private void OnEnable()
    {
        LevelManager.Ins.Pause();
        UIManager.Ins.GetUI(UIID.UICGamePlay).Close();
    }

    public void Restart()
    {
        LevelManager.Ins.Restart();
        //UIManager.Ins.OpenUI(UIID.UICGamePlay);
        //Close();
    }

    public void Menu()
    {
        LevelManager.Ins.BackToMenu();
        //UIManager.Ins.OpenUI(UIID.UICMainMenu);
        //Close();
    }
}

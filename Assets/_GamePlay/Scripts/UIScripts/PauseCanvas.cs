using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : UICanvas
{
    public void Restart()
    {
        UIManager.Ins.OpenUI(UIID.UICGamePlay);
        LevelManager.Ins.Restart();
        Close();
    }

    public void Resume()
    {
        UIManager.Ins.OpenUI(UIID.UICGamePlay);
        LevelManager.Ins.Resume();
        Close();
    }

    public void Menu()
    {
        UIManager.Ins.OpenUI(UIID.UICMainMenu);
        LevelManager.Ins.BackToMenu();
        Close();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCanvas : UICanvas
{
    public void Restart()
    {
        Close();
    }

    public void NextLevel()
    {
        Close();
    }

    public void Menu()
    {
        UIManager.Ins.OpenUI(UIID.UICMainMenu);
        Close();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailCanvas : UICanvas
{
    public void Restart()
    {
        Close();
    }

    public void Menu()
    {
        UIManager.Ins.OpenUI(UIID.UICMainMenu);
        Close();
    }
}

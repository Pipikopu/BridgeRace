using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainmenuCanvas : UICanvas
{
    public void PlayButton()
    {
        LevelManager.Ins.StartGame();
        //UIManager.Ins.OpenUI(UIID.UICGamePlay);
        //Close();
    }

    public void NewGameButton()
    {
        LevelManager.Ins.NewGame();
        //UIManager.Ins.OpenUI(UIID.UICGamePlay);
        //Close();
    }
}

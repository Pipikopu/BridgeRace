using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayCanvas : UICanvas
{
    public Text levelValue;

    private void Update()
    {
        levelValue.text = SceneManager.GetActiveScene().buildIndex.ToString();
    }

    public void Pause()
    {
        LevelManager.Ins.Pause();
        UIManager.Ins.OpenUI(UIID.UICPause);
        Close();
    }
}

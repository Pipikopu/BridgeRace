using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : UICanvas
{
    public Text countdownText;

    private void OnEnable()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        int i = 3;
        while (i >= 1)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
            i--;
        }
        Close();
        UIManager.Ins.OpenUI(UIID.UICGamePlay);
        Time.timeScale = 1;
        LevelManager.Ins.gameState = Constant.GameState.PLAY;
    }
}

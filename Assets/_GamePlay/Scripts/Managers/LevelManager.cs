using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void Restart()
    {
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMenu()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt(Constant.CURRENT_LEVEL_STRING, currentLevel);
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        int currentLevel = PlayerPrefs.GetInt(Constant.CURRENT_LEVEL_STRING);
        if (currentLevel == 0)
        {
            currentLevel = 1;
        }
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevel);
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt(Constant.CURRENT_LEVEL_STRING, 1);
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

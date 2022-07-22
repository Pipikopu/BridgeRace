using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void Restart()
    {
        SimplePool.ReleaseAll();
        UIManager.Ins.onOpenLoseGameMenu();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SimplePool.ReleaseAll();
        UIManager.Ins.onCloseEgameMenu();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMenu()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
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
        PlayerPrefs.SetInt("CurrentLevel", 1);
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

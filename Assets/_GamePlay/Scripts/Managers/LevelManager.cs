using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    public Constant.GameState gameState;

    private void Awake()
    {
        gameState = Constant.GameState.PLAY;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        Debug.Log(gameState);
    }

    public void Pause()
    {
        gameState = Constant.GameState.PAUSE;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        gameState = Constant.GameState.PLAY;
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        gameState = Constant.GameState.PLAY;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        gameState = Constant.GameState.PLAY;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMenu()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt(Constant.CURRENT_LEVEL_STRING, currentLevel);
        SimplePool.ReleaseAll();
        Time.timeScale = 1;
        gameState = Constant.GameState.PAUSE;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        int currentLevel = PlayerPrefs.GetInt(Constant.CURRENT_LEVEL_STRING);
        gameState = Constant.GameState.PLAY;
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
        gameState = Constant.GameState.PLAY;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

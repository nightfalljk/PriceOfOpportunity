using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject gameOverObject;

    private void Start()
    {
        playerController.GameOver += TriggerGameOver;
    }

    public void TriggerGameOver()
    {
        GameTimeManager.Instance.PauseGame();
        gameOverObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
